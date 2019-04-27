using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.UnitTests
{


    [TestFixture]
    public class TextSanitiserTests
    {
      
        private TextSanitiser _textSanitiser;

        [SetUp]
        public void Before_each_test()
        {
           _textSanitiser  = new TextSanitiser();
        }

        [TestCase("PENTEST<br><input type=\"Text\">", "PENTEST")]
        [TestCase("PENTEST 2<BR><input type=\"Text\"", "PENTEST 2")]
        [TestCase("PENTEST 3<BR<input type=\"Text\"", "PENTEST 3")]
        [TestCase("=PENTEST 4<BR<input type=\"Text\"", "PENTEST 4")]
        [TestCase("=PENTEST 5", "PENTEST 5")]
        [TestCase("==PENTEST 6====", "PENTEST 6")]

        public void CheckingHtmlIsStrippedOut(string inputText, string expectedOutput)
        {
            var result = _textSanitiser.SanitiseInputText(inputText);
            Assert.AreEqual(expectedOutput,result);
        }

       
    }
}
