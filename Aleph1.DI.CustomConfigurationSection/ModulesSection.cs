﻿using System;
using System.Configuration;

namespace Aleph1.DI.CustomConfigurationSection
{
	/// <summary>Custom Configuration Section for loading modules by the DI</summary>
	/// <seealso cref="ConfigurationSection" />
	public class ModulesSection : ConfigurationSection
	{
		/// <summary>path to the modules directory</summary>
		[ConfigurationProperty("base")]
		public BaseElement Base
		{
			get => (BaseElement)base["base"];
			set => base["base"] = value;
		}

		/// <summary>collection of modules elements</summary>
		[ConfigurationProperty("modules")]
		public ModuleElementCollection Modules
		{
			get => (ModuleElementCollection)base["modules"];
			set => base["modules"] = value;
		}
	}

	/// <summary>collection of modules elements</summary>
	/// <seealso cref="ConfigurationElementCollection" />
	[ConfigurationCollection(typeof(ModuleElement))]
	public class ModuleElementCollection : ConfigurationElementCollection
	{
		/// <summary>Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMapAlternate;

		/// <summary>Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.</summary>
		protected override string ElementName => "module";

		/// <summary>Indicates whether the specified <see cref="T:System.Configuration.ConfigurationElement" /> exists in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <param name="elementName">The name of the element to verify.</param>
		protected override bool IsElementName(string elementName)
		{
			return elementName.Equals("module", StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>Indicates whether the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object is read only.</summary>
		public override bool IsReadOnly()
		{
			return false;
		}

		/// <summary>When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ModuleElement();
		}

		/// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
		/// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ModuleElement)element).Path;
		}

		/// <summary>Gets the <see cref="ModuleElement"/> with the specified index.</summary>
		/// <value>The <see cref="ModuleElement"/>.</value>
		/// <param name="idx">The index.</param>
		public ModuleElement this[int idx] => (ModuleElement)BaseGet(idx);
	}

	/// <summary>a single Module to load</summary>
	/// <seealso cref="ConfigurationElement" />
	public class ModuleElement : ConfigurationElement
	{
		/// <summary>Gets the path to the module</summary>
		[ConfigurationProperty("path", IsRequired = true, IsKey = true)]
		public string Path => (string)base["path"];
	}

	/// <summary>path to the modules directory</summary>
	/// <seealso cref="ConfigurationElement" />
	public class BaseElement : ConfigurationElement
	{
		/// <summary>path to the modules directory</summary>
		[ConfigurationProperty("path", IsRequired = true, IsKey = true)]
		public string Path => (string)base["path"];
	}
}
