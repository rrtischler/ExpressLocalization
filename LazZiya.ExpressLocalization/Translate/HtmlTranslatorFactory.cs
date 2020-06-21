﻿using LazZiya.TranslationServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace LazZiya.ExpressLocalization.Translate
{
    /// <summary>
    /// HtmlTranslatorFactory
    /// </summary>
    public class HtmlTranslatorFactory : IHtmlTranslatorFactory
    {
        private readonly IOptions<ExpressLocalizationOptions> _options;
        private readonly ITranslationServiceFactory _translationServiceFactory;
        private readonly ILogger<HtmlTranslator> _logger;

        /// <summary>
        /// Initialize a new intance of HtmlTranslatorFactory
        /// </summary>
        /// <param name="options"></param>
        /// <param name="translationServiceFactory"></param>
        /// <param name="logger"></param>
        public HtmlTranslatorFactory(IOptions<ExpressLocalizationOptions> options, ITranslationServiceFactory translationServiceFactory, ILogger<HtmlTranslator> logger)
        {
            _options = options;
            _translationServiceFactory = translationServiceFactory;
            _logger = logger;
        }

        /// <summary>
        /// Create a new instance of HtmlTranslator
        /// </summary>
        /// <returns></returns>
        public IHtmlTranslator Create()
        {
            var tService = _translationServiceFactory.Create();
            return new HtmlTranslator(tService, _options, _logger);
        }
        
        /// <summary>
        /// Create a new instance of HtmlTranslator
        /// </summary>
        /// <returns></returns>
        public IHtmlTranslator Create(Type type)
        {
            if (type == null)
                throw new NullReferenceException(nameof(ITranslationService));
            
            if (type.GetInterface(typeof(ITranslationService).FullName) == null)
                throw new Exception($"The provided type is of type {type.FullName}, but this service must implement {typeof(ITranslationService)}");

            var tService = _translationServiceFactory.Create(type);
            return new HtmlTranslator(tService, _options, _logger);
        }
        
        /// <summary>
        /// Create new instance of IHtmlTranslator
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public IHtmlTranslator Create<TService>()
            where TService : ITranslationService
        {
            var tService = _translationServiceFactory.Create<TService>();
            return new HtmlTranslator(tService, _options, _logger);
        }
    }
}
