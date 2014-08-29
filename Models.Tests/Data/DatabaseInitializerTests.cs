using System;
using System.Linq;
using System.Linq.Expressions;
using DrunkAudible.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;

namespace DrunkAudible.Models.Tests
{
    [TestFixture]
    public class DatabaseInitializerTests
    {
        [Test]
        public void InitializeDatabaseWithSampleData ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();

            // Act
            DatabaseInitializer.Initialize (orm.Database);

            // Assert
            var authors = orm.Database.Table<Author> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.AuthorSamples,
                authors,
                new PublicPropertiesComparer<Author> ()
            );

            var series = orm.Database.Table<AudioSeries> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.SeriesSamples,
                series,
                new PublicPropertiesComparer<AudioSeries> ()
            );

            var episodes = orm.Database.Table<AudioEpisode> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.EpisodeSamples,
                episodes,
                new PublicPropertiesComparer<AudioEpisode> ()
            );

            var users = orm.Database.Table<User> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.SampleUsers,
                users,
                new PublicPropertiesComparer<User> ()
            );
        }
    }
}

