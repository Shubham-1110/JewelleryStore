using DinkToPdf;
using JewelleryStore.BusinessService.Interface;
using JewelleryStore.Common;
using JewelleryStore.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace JewelleryStore.BusinessService.Implementation
{
    public class PrintToFile : IPrint
    {
        private readonly IConfiguration _configuration;

        public PrintToFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Print(CalculatedPriceModel priceModel)
        {
            string fileStoragePath = _configuration.GetValue<string>(Constants.FileStoragePath);
            if (string.IsNullOrWhiteSpace(fileStoragePath))
            {
                throw new Exception(string.Format(ErrorMessageConstants.InvalidConfigurationKey, Constants.FileStoragePath));
            }

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

            var _converter = new SynchronizedConverter(new PdfTools());

            if (!Directory.Exists(fileStoragePath))
            {
                Directory.CreateDirectory(fileStoragePath);
            }

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = Path.Combine(fileStoragePath, "GoldPriceEstimation" + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_fffff") + ".pdf")
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HelperClass.GetHTMLString(priceModel),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            _converter.Convert(pdf);
        }
    }
    
}
