/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using Avro.Test.Utils;
using Avro;

namespace Avro.Test
{
    [TestFixture]
    public class SchemaNormalizationTests
    {
        [Test, TestCaseSource("ProvideCanonicalTestCases")]
        public void CanonicalTest(string input, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, SchemaNormalization.ToParsingForm(Schema.Parse(input)));
        }

        //[Test, TestCaseSource("ProvideFingerprintTestCases")]
        //public void FingerprintTest(string input, string expectedOutput)
        //{
        //    Schema s = Schema.Parse(input);
        //    Assert.AreEqual(expectedOutput, input);
        //}

        //private static IEnumerable<object> ProvideFingerprintTestCases()
        //{
        //    using (StreamReader reader = new StreamReader("../../../../../share/test/data/schema-tests.txt"))
        //    {
        //        return CaseFinder.Find(reader, "fingerprint", new List<object[]>());
        //    }
        //}

        private static IEnumerable<object> ProvideCanonicalTestCases()
        {
            using (StreamReader reader = new StreamReader("../../../../../share/test/data/schema-tests.txt"))
            {
                return CaseFinder.Find(reader, "canonical", new List<object[]>());
            }
        }
    }
}
