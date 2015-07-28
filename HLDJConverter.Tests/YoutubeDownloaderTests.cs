using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HLDJConverter.Tests
{
    [TestClass]
    public sealed class YoutubeDownloaderTests
    {
        [TestMethod]
        public void ShouldReturnVideoIDFromYoutubeLink()
        {
            var testCases = new[]
            {
                new {ExpectedID = "THBFWQIgWw4", Link = "https://www.youtube.com/watch?v=THBFWQIgWw4&list=PLRIWtICgwaX1gcSZ8qj8Q473tz7PsNmpR"},
                new {ExpectedID = "TQ_qwrrI8Og", Link = "https://www.youtube.com/watch?v=TQ_qwrrI8Og"},
                new {ExpectedID = "dPoMql9Ok9M", Link = "www.youtube.com/watch?v=dPoMql9Ok9M"},
                new {ExpectedID = "dPoMql9Ok9M", Link = "youtube.com/watch?v=dPoMql9Ok9M"},
                new {ExpectedID = "3q1JN_3s3gw", Link = "https://www.youtube.com/watch?v=3q1JN_3s3gw"},
                new {ExpectedID = "7wPH_Ed78VY", Link = "watch?v=7wPH_Ed78VY"},
                new {ExpectedID = "7wPH_Ed78VY", Link = "https://youtu.be/7wPH_Ed78VY"},
                new {ExpectedID = "iopENDI-Ayk", Link = "https://www.youtube.com/watch?v=iopENDI-Ayk"},
                new {ExpectedID = "6k_9QPAosTc", Link = "https://www.youtube.com/watch?v=6k_9QPAosTc"},
                new {ExpectedID = "6k_9QPAosTc", Link = "https://youtu.be/6k_9QPAosTc"},
                new {ExpectedID = "y_Rd2hByRyc", Link = "https://www.youtube.com/watch?feature=player_embedded&v=y_Rd2hByRyc"},
                new {ExpectedID = "-MPe7E1YrY4", Link = "https://www.youtube.com/watch?v=-MPe7E1YrY4&feature=player_embedded"},
                new {ExpectedID = "6xyQWw_j3fU", Link = "https://www.youtube.com/watch?v=6xyQWw_j3fU&feature=youtu.be"},
                new {ExpectedID = "0r7pnMVNvPE", Link = "https://www.youtube.com/watch?v=0r7pnMVNvPE"},
                new {ExpectedID = "3qy_kJbWfuI", Link = "https://www.youtube.com/watch?v=3qy_kJbWfuI"},
                new {ExpectedID = "n4R2p9WnzAo", Link = "https://www.youtube.com/watch?v=n4R2p9WnzAo"},
                new {ExpectedID = "BF_CI__LFSA", Link = "https://www.youtube.com/watch?v=BF_CI__LFSA"},
                new {ExpectedID = "LxO1Xqx7GqQ", Link = "https://www.youtube.com/watch?v=LxO1Xqx7GqQ"},
                new {ExpectedID = "obKI72j2iqo", Link = "https://www.youtube.com/watch?v=obKI72j2iqo&list=PL157A6EA7421FC089"},
                new {ExpectedID = "obKI72j2iqo", Link = "https://youtu.be/obKI72j2iqo?list=PL157A6EA7421FC089"},
            };

            foreach(var test in testCases)
            {
                Assert.AreEqual(test.ExpectedID, YoutubeDownloader.ExtractYoutubeID(test.Link));
            }
        }

        [TestMethod]
        public void ShouldReturnURLFromShortcut()
        {
            var testCases = new[]
            {
                new {ExpectedURL = "https://www.youtube.com/watch?v=p6j8fuvQICI", ShortcutFilepath = "TestShortcuts/Link1.url"},
                new {ExpectedURL = "https://www.youtube.com/watch?v=f9qavey27Kc", ShortcutFilepath = "TestShortcuts/Link2.url"},
                new {ExpectedURL = "https://www.youtube.com/watch?v=WpTOzX4xDUw", ShortcutFilepath = "TestShortcuts/Link3.url"},
                new {ExpectedURL = "https://www.youtube.com/watch?v=QGHlVLsbuoc", ShortcutFilepath = "TestShortcuts/Link4.url"},
            };

            foreach(var test in testCases)
            {
                Assert.AreEqual(test.ExpectedURL, YoutubeDownloader.ExtractURLFromShortcut(test.ShortcutFilepath));
            }
        }
    }
}
