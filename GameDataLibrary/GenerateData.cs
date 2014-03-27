/*
 * Copyright (c) 2011 BitSits Games
 *  
 * Shubhajit Saha    http://bitsits.blogspot.com/
 * Maya Agarwal      http://bitsitsgames.blogspot.com/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace GameDataLibrary
{
    public class GenerateData
    {
        public GenerateData()
        {
            object i = new object();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            //using (XmlWriter writer = XmlWriter.Create("Item.xml", settings))
            {
                //IntermediateSerializer.Serialize(writer, i, "Item.xml");
            }
        }
    }
}
