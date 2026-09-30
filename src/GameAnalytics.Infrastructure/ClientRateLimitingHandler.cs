using System.Threading.RateLimiting;

public class ClientRateLimitingHandler : DelegatingHandler
{
    private readonly RateLimiter _limiter;

    public ClientRateLimitingHandler(RateLimiter limiter)
    {
        _limiter = limiter;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Zde si bere 1 request. Pokud je vyčerpáno, vlákno se asynchronně 
        // zablokuje a čeká ve frontě.
        using var lease = await _limiter.AcquireAsync(1, cancellationToken);
        
        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Nastane jen pokud by se naplnila i čekací fronta (QueueLimit)
        return new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
    }
}