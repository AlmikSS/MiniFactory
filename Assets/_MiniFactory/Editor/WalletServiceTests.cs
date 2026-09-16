using MiniFactory.Gameplay.Economy;
using NUnit.Framework;
using UnityEngine;

namespace MiniFactory.Tests
{
    [TestFixture]
    public sealed class WalletServiceTests
    {
        private EconomyConfig _config;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<EconomyConfig>();
            var so = new UnityEditor.SerializedObject(_config);
            so.FindProperty("_startBalance").intValue = 100;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void TrySpend_EnoughBalance_ReturnsTrue()
        {
            var wallet = new WalletService(_config, null);
            var result = wallet.TrySpend(50);
            Assert.IsTrue(result);
            Assert.AreEqual(50, wallet.Balance);
        }

        [Test]
        public void TrySpend_ExactBalance_ReturnsTrue()
        {
            var wallet = new WalletService(_config, null);
            var result = wallet.TrySpend(100);
            Assert.IsTrue(result);
            Assert.AreEqual(0, wallet.Balance);
        }

        [Test]
        public void TrySpend_OneLessThanRequired_ReturnsFalse()
        {
            var wallet = new WalletService(_config, null);
            var result = wallet.TrySpend(101);
            Assert.IsFalse(result);
            Assert.AreEqual(100, wallet.Balance);
        }

        [Test]
        public void Add_NegativeOrZero_DoesNothing()
        {
            var wallet = new WalletService(_config, null);
            wallet.Add(0);
            wallet.Add(-10);
            Assert.AreEqual(100, wallet.Balance);
        }

        [Test]
        public void TrySpend_ZeroOrNegative_ReturnsTrueWithoutChange()
        {
            var wallet = new WalletService(_config, null);
            var result = wallet.TrySpend(0);
            Assert.IsTrue(result);
            Assert.AreEqual(100, wallet.Balance);
        }
    }
}