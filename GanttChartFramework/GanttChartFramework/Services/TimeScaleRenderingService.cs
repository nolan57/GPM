using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GanttChartFramework.Models;
using GanttChartFramework.Views;

namespace GanttChartFramework.Services
{
    /// <summary>
    /// High-performance rendering service for time scale components
    /// </summary>
    public class TimeScaleRenderingService
    {
        private readonly Dictionary<string, MultiLevelTimeScaleConfiguration> _configurationCache = new();
        private readonly Queue<ImprovedMultiLevelTimeScaleTick> _controlPool = new();
        private const int MaxPoolSize = 100;

        #region Object Pool Management

        /// <summary>
        /// Gets a time scale tick control from the pool or creates a new one
        /// </summary>
        public ImprovedMultiLevelTimeScaleTick GetTimeScaleTick()
        {
            if (_controlPool.Count > 0)
            {
                var control = _controlPool.Dequeue();
                ResetControl(control);
                return control;
            }

            return new ImprovedMultiLevelTimeScaleTick();
        }

        /// <summary>
        /// Returns a time scale tick control to the pool
        /// </summary>
        public void ReturnTimeScaleTick(ImprovedMultiLevelTimeScaleTick control)
        {
            if (_controlPool.Count < MaxPoolSize)
            {
                _controlPool.Enqueue(control);
            }
        }

        private void ResetControl(ImprovedMultiLevelTimeScaleTick control)
        {
            control.IsHighlighted = false;
            control.DateTime = DateTime.Now;
            control.Context = null;
        }

        #endregion

        #region Configuration Caching

        /// <summary>
        /// Gets or creates a cached configuration for the given parameters
        /// </summary>
        public MultiLevelTimeScaleConfiguration GetOrCreateConfiguration(
            TimeSpan timeSpan, 
            double availableWidth,
            string themeKey = "default")
        {
            var cacheKey = $"{timeSpan.TotalDays:F0}_{availableWidth:F0}_{themeKey}";
            
            if (_configurationCache.TryGetValue(cacheKey, out var cachedConfig))
            {
                return cachedConfig;
            }

            var config = CreateOptimizedConfiguration(timeSpan, availableWidth, themeKey);
            _configurationCache[cacheKey] = config;
            
            // Limit cache size
            if (_configurationCache.Count > 50)
            {
                var oldestKey = _configurationCache.Keys.First();
                _configurationCache.Remove(oldestKey);
            }

            return config;
        }

        private MultiLevelTimeScaleConfiguration CreateOptimizedConfiguration(
            TimeSpan timeSpan, 
            double availableWidth,
            string themeKey)
        {
            var config = MultiLevelTimeScaleConfiguration.CreateDefault();
            
            // Apply intelligent visibility rules based on time span
            var days = timeSpan.TotalDays;
            
            if (days <= 1)
            {
                // Hour/minute level detail
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Time).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Day).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.MaxRows = 2;
            }
            else if (days <= 7)
            {
                // Daily detail
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Day).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Week).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.MaxRows = 2;
            }
            else if (days <= 90)
            {
                // Weekly/Monthly detail
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Week).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Month).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.MaxRows = 3;
            }
            else if (days <= 365)
            {
                // Monthly/Quarterly detail
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Month).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Quarter).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.MaxRows = 3;
            }
            else
            {
                // Yearly/Quarterly detail
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Year).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.TimeLevels.Where(l => l.Type == TimeLevelType.Quarter).ToList()
                    .ForEach(l => l.IsVisible = true);
                config.MaxRows = 2;
            }

            // Apply theme
            ApplyTheme(config, themeKey);

            return config;
        }

        private void ApplyTheme(MultiLevelTimeScaleConfiguration config, string themeKey)
        {
            var theme = themeKey.ToLowerInvariant() switch
            {
                "dark" => CreateDarkTheme(),
                "light" => CreateLightTheme(),
                "modern" => TimeScaleTheme.CreateModern(),
                _ => new TimeScaleTheme()
            };

            foreach (var level in config.TimeLevels)
            {
                theme.Apply(level);
            }
        }

        #endregion

        #region Theme Factories

        private TimeScaleTheme CreateDarkTheme() => new TimeScaleTheme
        {
            PrimaryColor = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            SecondaryColor = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            AccentColor = new SolidColorBrush(Color.FromRgb(100, 149, 237)),
            BaseFontSize = 11.0
        };

        private TimeScaleTheme CreateLightTheme() => new TimeScaleTheme
        {
            PrimaryColor = new SolidColorBrush(Color.FromRgb(33, 37, 41)),
            SecondaryColor = new SolidColorBrush(Color.FromRgb(73, 80, 87)),
            AccentColor = new SolidColorBrush(Color.FromRgb(0, 123, 255)),
            BaseFontSize = 12.0
        };

        #endregion

        #region Batch Operations

        /// <summary>
        /// Creates multiple time scale ticks efficiently
        /// </summary>
        public List<ImprovedMultiLevelTimeScaleTick> CreateTimeScaleTicks(
            DateTime startDate,
            DateTime endDate,
            TimeSpan interval,
            double tickWidth,
            string themeKey = "default")
        {
            var ticks = new List<ImprovedMultiLevelTimeScaleTick>();
            var timeSpan = endDate - startDate;
            var totalTicks = (int)(timeSpan.Ticks / interval.Ticks);
            
            // Pre-allocate list capacity for better performance
            ticks.Capacity = totalTicks;

            var configuration = GetOrCreateConfiguration(timeSpan, tickWidth * totalTicks, themeKey);
            var context = new TimeScaleContext
            {
                StartDate = startDate,
                EndDate = endDate,
                AvailableWidth = tickWidth * totalTicks,
                AvailableHeight = 60
            };

            var currentDate = startDate;
            for (int i = 0; i < totalTicks; i++)
            {
                var tick = GetTimeScaleTick();
                tick.Configuration = configuration;
                tick.DateTime = currentDate;
                tick.Context = context;
                tick.Width = tickWidth;

                ticks.Add(tick);
                currentDate = currentDate.Add(interval);
            }

            return ticks;
        }

        /// <summary>
        /// Efficiently updates existing time scale ticks
        /// </summary>
        public void UpdateTimeScaleTicks(
            IEnumerable<ImprovedMultiLevelTimeScaleTick> ticks,
            MultiLevelTimeScaleConfiguration newConfiguration)
        {
            // Batch update to minimize UI thread calls
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                foreach (var tick in ticks)
                {
                    tick.Configuration = newConfiguration;
                }
            });
        }

        #endregion

        #region Memory Management

        /// <summary>
        /// Clears all caches to free memory
        /// </summary>
        public void ClearCaches()
        {
            _configurationCache.Clear();
            
            // Return all pooled controls for garbage collection
            _controlPool.Clear();
        }

        /// <summary>
        /// Gets memory usage statistics
        /// </summary>
        public TimeScaleMemoryStats GetMemoryStats()
        {
            return new TimeScaleMemoryStats
            {
                CachedConfigurations = _configurationCache.Count,
                PooledControls = _controlPool.Count,
                EstimatedMemoryUsage = CalculateEstimatedMemoryUsage()
            };
        }

        private long CalculateEstimatedMemoryUsage()
        {
            // Rough estimation in bytes
            var configCacheSize = _configurationCache.Count * 1024; // ~1KB per config
            var poolSize = _controlPool.Count * 2048; // ~2KB per control
            
            return configCacheSize + poolSize;
        }

        #endregion
    }

    /// <summary>
    /// Memory usage statistics for the time scale system
    /// </summary>
    public class TimeScaleMemoryStats
    {
        public int CachedConfigurations { get; set; }
        public int PooledControls { get; set; }
        public long EstimatedMemoryUsage { get; set; }

        public string FormattedMemoryUsage => 
            EstimatedMemoryUsage < 1024 * 1024 ? 
            $"{EstimatedMemoryUsage / 1024:F1} KB" : 
            $"{EstimatedMemoryUsage / (1024 * 1024):F1} MB";
    }
}
