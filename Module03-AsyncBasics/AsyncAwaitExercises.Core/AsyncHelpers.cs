namespace AsyncAwaitExercises.Core;

public class AsyncHelpers
{
    // Create a method that will try to get a response from a given `url`, retrying `maxTries` number of times.
    // It should wait one second before the second try, and double the wait time before every successive retry
    // (so pauses before retries will be 1, 2, 4, 8, ... seconds).
    // * `maxTries` must be at least 2
    // * we retry if:
    //    * we get non-successful status code (outside of 200-299 range), or
    //    * HTTP call thrown an exception (like network connectivity or DNS issue)
    // * token should be able to cancel both HTTP call and the retry delay
    // * if all retries fails, the method should throw the exception of the last try
    // HINTS:
    // * `HttpClient.GetStringAsync` does not accept cancellation token (use `GetAsync` instead)
    // * you may use `EnsureSuccessStatusCode()` method
    public static async Task<string> GetStringWithRetries(
        HttpClient client,
        string url,
        int maxTries = 3,
        CancellationToken token = default)
    {
        static async Task<string> GetString(HttpClient httpClient, string url, CancellationToken cancellationToken)
        {
            var resp = await httpClient.GetAsync(url, cancellationToken);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(cancellationToken);
        }

        if (maxTries < 2)
            throw new ArgumentException();

        int delayMs = 1000;
        // Try for the first (maxTries - 1) times
        while (maxTries-- > 1)
        {
            try
            {
                return await GetString(client, url, token);
            }
            catch (OperationCanceledException)
            {
                throw; // cancellation => rethrow to caller (no retry)
            }
            catch
            {
                await Task.Delay(delayMs, token);
                delayMs *= 2; // exponential retry delay
            }
        }

        // try for the last time
        return await GetString(client, url, token);
    }
}
