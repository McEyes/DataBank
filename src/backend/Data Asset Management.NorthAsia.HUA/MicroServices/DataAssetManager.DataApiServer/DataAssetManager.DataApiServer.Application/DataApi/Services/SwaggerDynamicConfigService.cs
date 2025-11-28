using Microsoft.OpenApi.Models;

public class SwaggerDynamicConfigService
{
    public List<OpenApiInfo> SwaggerDocs { get; } = new List<OpenApiInfo>();
    public List<(string Name, string Url)> SwaggerEndpoints { get; } = new List<(string, string)>();

    public void AddSwaggerDoc(string docName, OpenApiInfo info)
    {
        SwaggerDocs.Add(info);
    }

    public void AddSwaggerEndpoint(string name, string url)
    {
        SwaggerEndpoints.Add((name, url));
    }
}