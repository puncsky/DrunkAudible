// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using DrunkAudible.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Models.Tests
{
    [TestFixture]
    public class DatabaseInitializerTests
    {
        [Test]
        public void InitializeDatabaseWithSampleData_Author ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.Database.DropTable<Author> ();

            // Act
            DatabaseInitializer.Initialize (orm.Database);

            // Assert
            var authors = orm.Database.Table<Author> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.AuthorSamples,
                authors,
                new PublicPropertiesComparer<Author> ()
            );
        }

        [Test]
        public void InitializeDatabaseWithSampleData_Album ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.Database.DropTable<Album> ();

            // Act
            DatabaseInitializer.Initialize (orm.Database);

            // Assert
            var album = orm.Database.Table<Album> ().ToArray ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.AlbumSamples,
                album,
                new PublicPropertiesComparer<Album> (ignore: "Authors")
            );
        }

        [Test]
        public void InitializeDatabaseWithSampleData_AudioEpisodes ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.Database.DropTable<AudioEpisode> ();

            // Act
            DatabaseInitializer.Initialize (orm.Database);

            // Assert
            var episodes = orm.Database.Table<AudioEpisode> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.EpisodeSamples,
                episodes,
                new PublicPropertiesComparer<AudioEpisode> ("Authors", "Album", "IsDownloaded")
            );
        }

        [Test]
        public void InitializeDatabaseWithSampleData_Users ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.Database.DropTable<User> ();

            // Act
            DatabaseInitializer.Initialize (orm.Database);

            // Assert
            var users = orm.Database.Table<User> ();
            CollectionAssert.AreEqual (
                DatabaseInitializer.UserSamples,
                users,
                new PublicPropertiesComparer<User> ("AlbumFaves", "EpisodeFaves", "AuthorFaves")
            );
        }
    }
}

