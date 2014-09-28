// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using NUnit.Framework;
using DrunkAudible.Data;
using DrunkAudible.Data.Models;
using System.Linq;

namespace DrunkAudible.Mobile.Tests
{
    [TestFixture]
    public class ObjectRelationalMappingTests
    {
        [Test]
        public void InsertOrUpdate_Album_InsertNewAlbum ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.ClearAll ();
            var expectedAlbum = new Album {
                Id = 1,
                Title = "SampleTitle1",
                Authors = new[] { new Author { Id = 1, Name = "SampleAuthor1" } },
                Narrator = "SampleNarrator1",
                Episodes = new[] {
                    new AudioEpisode { Id = 1, Title = "Episode1" },
                    new AudioEpisode { Id = 2, Title = "Episode2" },
                },
            };

            // Act
            orm.InsertOrUpdate (expectedAlbum);

            // Assert
            Assert.AreEqual (1, orm.Albums.Length);
            var actualAlbum = orm.Albums.FirstOrDefault ();
            Assert.AreEqual (
                0,
                new PublicPropertiesComparer<Album> ("Authors", "Episodes").Compare(expectedAlbum, actualAlbum)
            );
            CollectionAssert.AreEqual (
                expectedAlbum.Authors,
                actualAlbum.Authors,
                new PublicPropertiesComparer<Author> ()
            );
            CollectionAssert.AreEqual (
                expectedAlbum.Episodes,
                actualAlbum.Episodes,
                new PublicPropertiesComparer<Author> ()
            );
        }

        [Test]
        public void InsertOrUpdate_Album_UpdateExistingAlbum ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.ClearAll ();
            var existingAlbum = new Album {
                Id = 1,
                Title = "SampleTitle1",
                Authors = new[] { new Author { Id = 1, Name = "SampleAuthor1" } },
                Narrator = "SampleNarrator1",
                Episodes = new[] {
                    new AudioEpisode { Id = 1, Title = "Episode1" },
                    new AudioEpisode { Id = 2, Title = "Episode2" },
                },
            };
            var expectedAlbum = new Album {
                Id = 1,
                Title = "SampleTitle2",
                Authors = new[] { new Author { Id = 2, Name = "SampleAuthor2" } },
                Narrator = "SampleNarrator2",
                Episodes = new[] {
                    new AudioEpisode { Id = 1, Title = "Episode1" },
                    new AudioEpisode { Id = 2, Title = "Episode2" },
                },
            };
            orm.InsertOrUpdate (existingAlbum);

            // Act
            orm.InsertOrUpdate (expectedAlbum);

            // Assert
            Assert.AreEqual (1, orm.Albums.Length);
            var actualAlbum = orm.Albums.FirstOrDefault ();
            Assert.AreEqual (
                0,
                new PublicPropertiesComparer<Album> ("Authors", "Episodes").Compare(expectedAlbum, actualAlbum)
            );
            CollectionAssert.AreEqual (
                expectedAlbum.Authors,
                actualAlbum.Authors,
                new PublicPropertiesComparer<Author> ()
            );
            CollectionAssert.AreEqual (
                expectedAlbum.Episodes,
                actualAlbum.Episodes,
                new PublicPropertiesComparer<AudioEpisode> ()
            );
        }

        [Test]
        public void InsertOrUpdate_Albums_InsertNewAlbums ()
        {
            // Arrange
            var orm = new ObjectRelationalMapping ();
            orm.ClearAll ();
            var expectedAlbums = new Album[] {
                new Album {
                    Id = 1,
                    Title = "SampleTitle1",
                    Authors = new[] { new Author { Id = 1, Name = "SampleAuthor1" } },
                    Narrator = "SampleNarrator1",
                    Episodes = new[] {
                        new AudioEpisode { Id = 1, Title = "Episode1" },
                        new AudioEpisode { Id = 2, Title = "Episode2" },
                    },
                },
                new Album {
                    Id = 2,
                    Title = "SampleTitle2",
                    Authors = new[] { new Author { Id = 1, Name = "SampleAuthor1" } },
                    Narrator = "SampleAuthor1",
                    Episodes = new[] {
                        new AudioEpisode { Id = 1, Title = "Episode1" },
                        new AudioEpisode { Id = 2, Title = "Episode2" },
                    },
                },
            };

            // Act
            orm.InsertOrUpdate (expectedAlbums);

            // Assert
            Assert.AreEqual (2, orm.Albums.Length);
            var actualAlbums = orm.Albums;
            CollectionAssert.AreEqual (
                expectedAlbums,
                actualAlbums,
                new PublicPropertiesComparer<Album> ("Authors", "Episodes")
            );
            for (int i = 0; i < orm.Albums.Length; i++) {
                CollectionAssert.AreEqual (
                    expectedAlbums [i].Authors,
                    actualAlbums [i].Authors,
                    new PublicPropertiesComparer<Author> ()
                );
            }
            for (int i = 0; i < orm.Albums.Length; i++) {
                CollectionAssert.AreEqual (
                    expectedAlbums [i].Episodes,
                    actualAlbums [i].Episodes,
                    new PublicPropertiesComparer<AudioEpisode> ()
                );
            }
        }
    }
}

