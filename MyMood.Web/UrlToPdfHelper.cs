using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EvoPdf.HtmlToPdf;

namespace MyMood.Web
{
    public class UrlToPdfHelper
    {
        #region ToPdfBytes
        public static byte[] ToPdfBytes(string urlToConvert, PdfPageOrientation pageOrientation = PdfPageOrientation.Portrait)
        {
            // Create the PDF converter. Optionally the HTML viewer width can be specified as parameter
            // The default HTML viewer width is 1024 pixels.
            PdfConverter pdfConverter = new PdfConverter();

            // set the license key - required
            pdfConverter.LicenseKey = "1f7n9ebm9eTl5uP15vvl9ebk++Tn++zs7Ow=";

            // set the converter options - optional
            pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Best;
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = pageOrientation;
            pdfConverter.PdfDocumentOptions.TopMargin = 20;
            pdfConverter.PdfDocumentOptions.BottomMargin = 20;

            // set if the HTML content is resized if necessary to fit the PDF page width - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;

            // set the embedded fonts option - optional - default is false
            pdfConverter.PdfDocumentOptions.EmbedFonts = true;

            // set if the JavaScript is enabled during conversion to a PDF - default  is true
            pdfConverter.JavaScriptEnabled = true;

            // set if the images in PDF are compressed with JPEG to reduce the  PDF document size - default is true
            pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;

            pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = false;
            pdfConverter.PdfDocumentOptions.InternalLinksEnabled = true;
            pdfConverter.ConversionDelay = 1;

            // Performs the conversion and get the pdf document bytes that can

            // be saved to a file or sent as a browser response
            return pdfConverter.GetPdfBytesFromUrl(urlToConvert);
        }
        #endregion
    }
}