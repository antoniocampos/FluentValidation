#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Resources {
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Globalization;
	using Validators;

	/// <summary>
	/// Allows the default error message translations to be managed.
	/// </summary>
	public class LanguageManager : ILanguageManager {
		private readonly ConcurrentDictionary<string, Language> _languages;
		private readonly Language _fallback = new EnglishLanguage();

		/// <summary>
		/// Creates a new instance of the LanguageManager class.
		/// </summary>
		public LanguageManager() {
			// Initialize with English as the default. Others will be lazily loaded as needed.
			_languages = new ConcurrentDictionary<string, Language>(new[] {
				new KeyValuePair<string, Language>(EnglishLanguage.Culture, _fallback),
			});
		}
		
		/// <summary>
		/// Language factory.
		/// </summary>
		/// <param name="culture">The culture code.</param>
		/// <returns>The corresponding Language instance or null.</returns>
		private static Language CreateLanguage(string culture) {
			switch (culture) {
				case EnglishLanguage.Culture: return new EnglishLanguage();
				case AlbanianLanguage.Culture: return new AlbanianLanguage();
				case ArabicLanguage.Culture: return new ArabicLanguage();
				case ChineseSimplifiedLanguage.Culture: return new ChineseSimplifiedLanguage();
				case ChineseTraditionalLanguage.Culture: return new ChineseTraditionalLanguage();
				case CroatianLanguage.Culture: return new CroatianLanguage();
				case CzechLanguage.Culture: return new CzechLanguage();
				case DanishLanguage.Culture: return new DanishLanguage();
				case DutchLanguage.Culture: return new DutchLanguage();
				case FinnishLanguage.Culture: return new FinnishLanguage();
				case FrenchLanguage.Culture: return new FrenchLanguage();
				case GermanLanguage.Culture: return new GermanLanguage();
				case GeorgianLanguage.Culture: return new GeorgianLanguage();
				case GreekLanguage.Culture: return new GreekLanguage();
				case HebrewLanguage.Culture: return new HebrewLanguage();
				case HindiLanguage.Culture: return new HindiLanguage();
				case ItalianLanguage.Culture: return new ItalianLanguage();
				case JapaneseLanguage.Culture: return new JapaneseLanguage();
				case KoreanLanguage.Culture: return new KoreanLanguage();
				case MacedonianLanguage.Culture: return new MacedonianLanguage();
				case NorwegianBokmalLanguage.Culture: return new NorwegianBokmalLanguage();
				case PersianLanguage.Culture: return new PersianLanguage();
				case PolishLanguage.Culture: return new PolishLanguage();
				case PortugueseLanguage.Culture: return new PortugueseLanguage();
				case PortugueseBrazilLanguage.Culture: return new PortugueseBrazilLanguage();
				case RomanianLanguage.Culture: return new RomanianLanguage();
				case RussianLanguage.Culture: return new RussianLanguage();
				case SlovakLanguage.Culture: return new SlovakLanguage();
				case SpanishLanguage.Culture: return new SpanishLanguage();
				case SwedishLanguage.Culture: return new SwedishLanguage();
				case TurkishLanguage.Culture: return new TurkishLanguage();
				case UkrainianLanguage.Culture: return new UkrainianLanguage();
				default: return null;
			}
		}


		/// <summary>
		/// Whether localization is enabled.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Default culture to use for all requests to the LanguageManager. If not specified, uses the current UI culture.
		/// </summary>
		public CultureInfo Culture { get; set; }

		/// <summary>
		/// Removes all languages except the default.
		/// </summary>
		public void Clear() {
			_languages.Clear();
		}

		/// <summary>
		/// Gets a translated string based on its key. If the culture is specific and it isn't registered, we try the neutral culture instead.
		/// If no matching culture is found  to be registered we use English.
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="culture">The culture to translate into</param>
		/// <returns></returns>
		public virtual string GetString(string key, CultureInfo culture=null) {
			culture = culture ?? Culture ?? CultureInfo.CurrentUICulture;

			var languageToUse = GetCachedLanguage(culture);
			string value = languageToUse.GetTranslation(key);

			// Selected language is missing a translation for this key - fall back to English translation. 
			if (string.IsNullOrEmpty(value) && languageToUse != _fallback) {
				value = _fallback.GetTranslation(key);
			}

			return value ?? string.Empty;
		}

		private Language GetCachedLanguage(CultureInfo culture) {
			// If the language manager is enabled, try and find matching translations.
			if (Enabled) {
				var languageToUse = _languages.GetOrAdd(culture.Name, CreateLanguage);

				// If we couldn't find translations for this culture, and it's not a neutral culture
				// then try and find translations for the parent culture instead.
				if (languageToUse == null && !culture.IsNeutralCulture) {
					languageToUse = _languages.GetOrAdd(culture.Parent.Name, CreateLanguage);
				}

				if (languageToUse != null) {
					return languageToUse;
				}
			}

			return _fallback;
		}

		public IEnumerable<string> GetSupportedTranslationKeys() {
			return _fallback.GetSupportedKeys();
		}

		public void AddTranslation(string language, string key, string message) {
			if(string.IsNullOrEmpty(language)) throw new ArgumentNullException(nameof(language));
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

			if (!_languages.ContainsKey(language)) {
				_languages[language] = new GenericLanguage(language);
			}

			_languages[language].Translate(key, message);
		}

	}
}
