﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Nikse.SubtitleEdit.Logic;
using System.Collections.Generic;

namespace Nikse.SubtitleEdit.Forms
{
    public sealed partial class ChooseLanguage : Form
    {
        private const string CustomLanguageFileName = "Language.xml";

        public class CultureListItem
        {
            CultureInfo _cultureInfo;

            public CultureListItem(CultureInfo cultureInfo)
            {
                _cultureInfo = cultureInfo;
            }

            public override string ToString()
            {
                return _cultureInfo.NativeName;
            }

            public string Name
            {
                get { return _cultureInfo.Name; }
            }
        }

        public string CultureName
        {
            get 
            { 
                int index = comboBoxLanguages.SelectedIndex;
                if (index == -1)
                    return "en-US";
                else if (comboBoxLanguages.Items[index].ToString() == CustomLanguageFileName)
                    return CustomLanguageFileName;
                else
                    return (comboBoxLanguages.Items[index] as CultureListItem).Name;
            }
        }

        public ChooseLanguage()
        {
            InitializeComponent();

            List<string> list = new List<string>();
            foreach (string name in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                string prefix = "Nikse.SubtitleEdit.Resources.";
                string postfix = ".xml.zip";
                if (name.StartsWith(prefix) && name.EndsWith(postfix))
                {
                    string cultureName = name.Substring(prefix.Length, name.Length - prefix.Length - postfix.Length);
                    list.Add(cultureName);
                }
            }
            list.Sort();
            comboBoxLanguages.Items.Add(new CultureListItem(CultureInfo.CreateSpecificCulture("en-US")));
            foreach (string cultureName in list)
            {
                try
                {
                    comboBoxLanguages.Items.Add(new CultureListItem(CultureInfo.CreateSpecificCulture(cultureName)));
                }
                catch (ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine(cultureName + " is not a valid culture");
                }
            }
           
            int index = 0;
            for (int i=0; i< comboBoxLanguages.Items.Count; i++)
            {
                var item = (CultureListItem)comboBoxLanguages.Items[i];
                if (item.Name == Configuration.Settings.Language.General.CultureName)
                    index = i;
            }
            comboBoxLanguages.SelectedIndex = index;

            string customLanguageFile = Path.Combine(Configuration.BaseDirectory, CustomLanguageFileName);
            if (File.Exists(customLanguageFile))
            {
                comboBoxLanguages.Items.Add(CustomLanguageFileName);
            }


            Text = Configuration.Settings.Language.ChooseLanguage.Title;
            labelLanguage.Text = Configuration.Settings.Language.ChooseLanguage.Language;
            buttonOK.Text = Configuration.Settings.Language.General.OK;
            buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        private void ChangeLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                DialogResult = DialogResult.Cancel;
            else if (e.Shift && e.Control && e.Alt && e.KeyCode == Keys.L)
            {
                Configuration.Settings.Language.Save();
                Configuration.Settings.Language.SaveAndCompress();
            }
            else if (e.Shift && e.Control && e.Alt && e.KeyCode == Keys.C)
                CompareTags();
        }

        private static void CompareTags()
        {
            Configuration.Settings.Language.CompareWithEnglish();
        }
    }
}
