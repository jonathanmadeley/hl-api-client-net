using HL.Client.Entities;
using NUnit.Framework;

namespace HL.Client.Test
{
    /// <summary>
    /// Assert different <see cref="HL"/> entities are equal to each other
    /// </summary>
    internal static class EntitiesAssert
    {
        /// <summary>
        /// Check two <see cref="TransactionEntity"/> objects are equal
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreEqual(TransactionEntity expected, TransactionEntity actual)
        {
            Assert.That(actual.TradeDate, Is.EqualTo(expected.TradeDate), nameof(actual.TradeDate));
            Assert.That(actual.SettleDate, Is.EqualTo(expected.SettleDate), nameof(actual.SettleDate));
            Assert.That(actual.Reference, Is.EqualTo(expected.Reference), nameof(actual.Reference));
            Assert.That(actual.ReferenceLink, Is.EqualTo(expected.ReferenceLink), nameof(actual.ReferenceLink));
            Assert.That(actual.Description, Is.EqualTo(expected.Description), nameof(actual.Description));
            Assert.That(actual.UnitCost, Is.EqualTo(expected.UnitCost), nameof(actual.UnitCost));
            Assert.That(actual.Quantity, Is.EqualTo(expected.Quantity), nameof(actual.Quantity));
            Assert.That(actual.Value, Is.EqualTo(expected.Value), nameof(actual.Value));
        }

        /// <summary>
        /// Check two <see cref="CashSummaryEntity"/> objects are equal
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreEqual(CashSummaryEntity expected, CashSummaryEntity actual)
        {
            Assert.That(actual.CashOnCapitalAccount, Is.EqualTo(expected.CashOnCapitalAccount), nameof(actual.CashOnCapitalAccount));
            Assert.That(actual.IncomeLoyaltyBonus, Is.EqualTo(expected.IncomeLoyaltyBonus), nameof(actual.IncomeLoyaltyBonus));
            Assert.That(actual.FixedRateOffers, Is.EqualTo(expected.FixedRateOffers), nameof(actual.FixedRateOffers));
            Assert.That(actual.TotalCash, Is.EqualTo(expected.TotalCash), nameof(actual.TotalCash));
        }

        /// <summary>
        /// Check two <see cref="StockEntity"/> objects are equal
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreEqual(StockEntity expected, StockEntity actual)
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id), nameof(actual.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name), nameof(actual.Name));
            Assert.That(actual.UnitType, Is.EqualTo(expected.UnitType), nameof(actual.UnitType));
            Assert.That(actual.UnitsHeld, Is.EqualTo(expected.UnitsHeld), nameof(actual.UnitsHeld));
            Assert.That(actual.Price, Is.EqualTo(expected.Price), nameof(actual.Price));
            Assert.That(actual.Value, Is.EqualTo(expected.Value), nameof(actual.Value));
            Assert.That(actual.Cost, Is.EqualTo(expected.Cost), nameof(actual.Cost));
            AreEqual(expected.GainsLoss, actual.GainsLoss);
        }

        /// <summary>
        /// Check two <see cref="GainsLossEntity"/> objects are equal
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreEqual(GainsLossEntity expected, GainsLossEntity actual)
        {
            Assert.That(actual.Pounds, Is.EqualTo(expected.Pounds), nameof(actual.Pounds));
            Assert.That(actual.Percentage, Is.EqualTo(expected.Percentage), nameof(actual.Percentage));
        }

        /// <summary>
        /// Check two <see cref="AccountEntity"/> objects are equal
        /// </summary>
        /// <param name="expected">The expected object</param>
        /// <param name="actual">The actual object</param>
        public static void AreEqual(AccountEntity expected, AccountEntity actual)
        {
            Assert.That(actual.Id, Is.EqualTo(expected.Id), nameof(actual.Id));
            Assert.That(actual.Name, Is.EqualTo(expected.Name), nameof(actual.Name));
            Assert.That(actual.StockValue, Is.EqualTo(expected.StockValue), nameof(actual.StockValue));
            Assert.That(actual.CashValue, Is.EqualTo(expected.CashValue), nameof(actual.CashValue));
            Assert.That(actual.TotalValue, Is.EqualTo(expected.TotalValue), nameof(actual.TotalValue));
            Assert.That(actual.Available, Is.EqualTo(expected.Available), nameof(actual.Available));
        }
    }
}
