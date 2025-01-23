using Microsoft.Extensions.DependencyInjection;

namespace BlazorPdf
{
    public static class Registry
    {
        public static IServiceCollection RegisterBlazorPdf(this IServiceCollection services)
        {
            services.AddScoped<PdfViewerJsInterop>();
            return services;
        }
    }
}
