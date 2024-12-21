using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using Loaf.Models;
using Microsoft.Extensions.Configuration;

namespace Loaf.Config
{
    public class JsonConfigManager
    {
        private readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public JsonConfigManager()
        {
            ConfigurationBuilder configBuilder = new();
            configBuilder.AddJsonFile(_configPath, true, true);
            IConfigurationRoot configurationRoot = configBuilder.Build();
            ConfigModel = configurationRoot.Get<ConfigModel>() ?? new();
            ConfigModel.PropertyChanged += SaveConfig;
        }

        public ConfigModel ConfigModel { get; }

        private void SaveConfig()
        {
            string jsonString = JsonSerializer.Serialize(ConfigModel);
            try
            {
                File.WriteAllText(_configPath, jsonString);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"无法保存配置:{ex}", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"未知错误:{ex}", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}