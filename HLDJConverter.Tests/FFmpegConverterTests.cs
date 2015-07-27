using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HLDJConverter.Tests
{
    [TestClass]
    public sealed class FFmpegConverterTests
    {
        [TestMethod]
        public void ShouldReturnValidFilename()
        {
            var testCases = new[]
            {
                new { ValidFilename = "Touhou PV MariAliCANDY",                                 Filename = "【Touhou PV】 MariAli☆CANDY 【東方音遊戯】"},
                new { ValidFilename = "Hatsune Miku - Unfragment [P]",                          Filename = "Hatsune Miku - Unfragment [鼻そうめんP]"},
                new { ValidFilename = "[Remake] Chameleon washing his hands",                   Filename = "[Remake] Chameleon washing his hands  Хамелеон моет лапки"},
                new { ValidFilename = "YG - My Nigga (Explicit) ft. Jeezy, Rich Homie Quan",    Filename = "YG - My Nigga (Explicit) ft. Jeezy, Rich Homie Quan"},
                new { ValidFilename = "Dance. ParagonX9___HyperioxX",                           Filename = "Dance. ParagonX9___HyperioxX"},
                new { ValidFilename = "#IllyaDance DANCE  ILLYA",                               Filename = "#IllyaDance DANCE ☆ ILLYA ☆☆☆"},
                new { ValidFilename = "kannkore Kantai Collection",                             Filename = "こんごうぱーく kannkore Kantai Collection"},
                new { ValidFilename = "MV Full ver.",                                           Filename = "임창정-문을 여시오 M/V Full ver."},
                new { ValidFilename = "I'm Gonna Build A Base",                                 Filename = "♪ I'm Gonna Build A Base"},
                new { ValidFilename = "(HD)TV    Tamako Market ED",                             Filename = "(HD)【TV アニメ】 たまこまーけっと / Tamako Market ED"},
                new { ValidFilename = "MW3 xXLeGiTqUiCkSCOPEzzXx [[MLG]] ~360~ ..",             Filename = "MW3 xXLeGiTqUiCkSCOPEzzXx [[MLG]] ~360~ .::."},
            };

            foreach(var test in testCases)
            {
                Assert.AreEqual(test.ValidFilename, FFmpegConverter.RemoveInvalidFilenameCharacters(test.Filename));
            }
        }
    }
}
