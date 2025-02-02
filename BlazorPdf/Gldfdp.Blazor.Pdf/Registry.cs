using Microsoft.Extensions.DependencyInjection;

namespace Gldfdp.Blazor.Pdf
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
