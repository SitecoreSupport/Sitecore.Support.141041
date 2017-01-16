using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;
using System.Web;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Configuration;
namespace Sitecore.Support.Pipelines.HttpRequest
{
    public class LanguageResolver : Sitecore.Pipelines.HttpRequest.LanguageResolver
    {
        public override void Process(HttpRequestArgs args)
        {
            Language result;
            Assert.ArgumentNotNull(args, "args");
            Language languageFromRequest = this.GetLanguageFromRequest(args.Context.Request);
            if (languageFromRequest == null)
            {
                if (args.Context.Request.Cookies["sc_lang"] == null || String.IsNullOrEmpty(args.Context.Request.Cookies["sc_lang"].Value))
                {
                    return;
                }
                Language.TryParse(args.Context.Request.Cookies["sc_lang"].Value, out result);
                languageFromRequest = result;
                if (languageFromRequest == null)
                {
                    Language.TryParse(Settings.DefaultLanguage, out result);
                    Context.Language = result;
                    return;
                }
            }
            Context.Language = languageFromRequest;
            Tracer.Info("Language changed to \"" + languageFromRequest.Name + "\" as the query string of the current request contains language (sc_lang).");
        }

        /// <summary>
        /// Extracts the language name from the query string.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private Language ExtractLanguageFromQueryString(System.Web.HttpRequest request)
        {
            string text = request.QueryString["sc_lang"];
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            Language result;
            if (!Language.TryParse(text, out result))
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Gets the language from request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private Language GetLanguageFromRequest(System.Web.HttpRequest request)
        {
            Language language = this.ExtractLanguageFromQueryString(request);
            if (language != null)
            {
                return language;
            }
            return Context.Data.FilePathLanguage;
        }


    }
}