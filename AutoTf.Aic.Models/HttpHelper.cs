namespace AutoTf.Aic.Models;

public static class HttpHelper
{
    /// <summary>
    /// Sends a GET request to the given endpoint and returns it's content as a string.
    /// </summary>
    public static async Task<string> SendGetString(string endpoint, bool reThrow = true)
    {
        try
        {
            using HttpClient client = new HttpClient();
			
            HttpResponseMessage response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            if(reThrow)
                throw;
            return "";
        }
    }

    public static async Task SendPost(string endpoint, HttpContent content, bool reThrow = true)
    {
        try
        {
            using HttpClient client = new HttpClient();
			
            HttpResponseMessage response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            if(reThrow)
                throw;
        }
    }
}