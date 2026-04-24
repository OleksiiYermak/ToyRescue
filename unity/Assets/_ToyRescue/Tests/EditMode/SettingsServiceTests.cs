using NUnit.Framework;
using ToyRescue.Services;
using UnityEngine;

namespace ToyRescue.Tests
{
    public sealed class SettingsServiceTests
    {
        [Test]
        public void DefaultsToMusicAndSfxEnabled()
        {
            string keyPrefix = CreateUniquePrefix();

            try
            {
                var service = new SettingsService(keyPrefix: keyPrefix);

                Assert.IsTrue(service.MusicEnabled);
                Assert.IsTrue(service.SfxEnabled);
            }
            finally
            {
                DeleteKeys(keyPrefix);
            }
        }

        [Test]
        public void PersistsValuesAcrossServiceInstances()
        {
            string keyPrefix = CreateUniquePrefix();

            try
            {
                var writer = new SettingsService(keyPrefix: keyPrefix);
                writer.SetMusicEnabled(false);
                writer.SetSfxEnabled(false);

                var reader = new SettingsService(keyPrefix: keyPrefix);

                Assert.IsFalse(reader.MusicEnabled);
                Assert.IsFalse(reader.SfxEnabled);
            }
            finally
            {
                DeleteKeys(keyPrefix);
            }
        }

        [Test]
        public void DisablingMusicPublishesChangeEvent()
        {
            string keyPrefix = CreateUniquePrefix();

            try
            {
                var service = new SettingsService(keyPrefix: keyPrefix);
                bool wasRaised = false;

                service.SettingsChanged += eventData =>
                {
                    wasRaised = true;
                    Assert.IsFalse(eventData.MusicEnabled);
                    Assert.IsTrue(eventData.SfxEnabled);
                };

                service.SetMusicEnabled(false);

                Assert.IsTrue(wasRaised);
            }
            finally
            {
                DeleteKeys(keyPrefix);
            }
        }

        private static string CreateUniquePrefix()
        {
            return $"ToyRescue.Tests.Settings.{System.Guid.NewGuid():N}";
        }

        private static void DeleteKeys(string keyPrefix)
        {
            PlayerPrefs.DeleteKey($"{keyPrefix}.MusicEnabled");
            PlayerPrefs.DeleteKey($"{keyPrefix}.SfxEnabled");
            PlayerPrefs.Save();
        }
    }
}
