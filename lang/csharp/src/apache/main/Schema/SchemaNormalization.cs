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

using System.Collections.Generic;
using System.Text;

namespace Avro
{
    /// <summary>
    /// Collection of static methods for generating the cannonical form of schemas.
    /// </summary>
    public static class SchemaNormalization
    {
        /// <summary>
        /// Parses a schema into the canonical form as defined by Avro spec.
        /// </summary>
        /// <param name="s">Schema</param>
        /// <returns>Parsing Canonical Form of a schema as defined by Avro spec.</returns>
        public static string ToParsingForm(Schema s)
        {
            IDictionary<string, string> env = new Dictionary<string, string>();
            return Build(env, s, new StringBuilder()).ToString();
        }

        private static StringBuilder Build(IDictionary<string, string> env, Schema s, StringBuilder o)
        {
            bool firstTime = true;
            Schema.Type st = s.Tag;
            switch (st)
            {
                case Schema.Type.Union:
                    UnionSchema us = s as UnionSchema;
                    o.Append('[');
                    foreach(Schema b in us.Schemas)
                    {
                        if (!firstTime)
                        {
                            o.Append(",");
                        }
                        else
                        {
                            firstTime = false;
                        }
                        Build(env, b, o);
                    }
                    return o.Append(']');

                case Schema.Type.Array:
                case Schema.Type.Map:
                    o.Append("{\"type\":\"").Append(Schema.GetTypeString(s.Tag)).Append("\"");
                    if (st == Schema.Type.Array)
                    {
                        ArraySchema arraySchema  = s as ArraySchema;
                        Build(env, arraySchema.ItemSchema, o.Append(",\"items\":"));
                    }
                    else
                    {
                        MapSchema mapSchema = s as MapSchema;
                        Build(env, mapSchema.ValueSchema, o.Append(",\"values\":"));
                    }
                    return o.Append("}");

                case Schema.Type.Enumeration:
                case Schema.Type.Fixed:
                case Schema.Type.Record:
                    NamedSchema namedSchema = s as NamedSchema;
                    var name = namedSchema.Fullname;
                    if (env.ContainsKey(name))
                    {
                        return o.Append(env[name]);
                    }
                    var qname = "\"" + name + "\"";
                    env.Add(name, qname);
                    o.Append("{\"name\":").Append(qname);
                    o.Append(",\"type\":\"").Append(Schema.GetTypeString(s.Tag)).Append("\"");
                    if (st == Schema.Type.Enumeration)
                    {
                        EnumSchema enumSchema = s as EnumSchema;
                        o.Append(",\"symbols\":[");
                        foreach (var enumSymbol in enumSchema.Symbols)
                        {
                            if (!firstTime)
                            {
                                o.Append(",");
                            }
                            else
                            {
                                firstTime = false;
                            }
                            o.Append("\"").Append(enumSymbol).Append("\"");
                        }
                        o.Append("]");
                    }
                    else if (st == Schema.Type.Fixed)
                    {
                        FixedSchema fixedSchema = s as FixedSchema;
                        o.Append(",\"size\":").Append(fixedSchema.Size.ToString());
                    }
                    else  // st == Schema.Type.Record
                    {
                        RecordSchema recordSchema = s as RecordSchema;
                        o.Append(",\"fields\":[");
                        foreach (var field in recordSchema.Fields)
                        {
                            if (!firstTime)
                            {
                                o.Append(",");
                            }
                            else
                            {
                                firstTime = false;
                            }
                            o.Append("{\"name\":\"").Append(field.Name).Append("\"");
                            Build(env, field.Schema, o.Append(",\"type\":")).Append("}");
                        }
                        o.Append("]");
                    }
                    return o.Append("}");

                default:    //boolean, bytes, double, float, int, long, null, string
                    return o.Append("\"").Append(s.Name).Append("\"");
            }
        }
    }
}
