﻿using GetByNameLibrary.Domains;
using HtmlAgilityPack;
using SimpleLogger;
using System;
using System.Collections.Generic;

namespace GetByNameLibrary.Stores
{
    public class Directcod : Store
    {
        public Directcod()
        {
            FileName = "directcod";
            StoreUrl = "http://direct.cod.ru";
            PageCount = 0;
            PageUrl = "http://direct.cod.ru/goods/";

        }

        public override Boolean StartParse()
        {
            var result = true;
            try
            {
                this.Parse(GetPage(PageUrl));
                this.SaveEntries();
            }
            catch (Exception ex) 
            {
                result = false;
                _logger.AddEntry(ex.ToString(), MessageType.Error);
                _logger.WriteLogs();
            }
            return result;
        }

        protected override void Parse(string page)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='goodListSml double']");

            if (nodes != null)
            {
                var names = new List<String>();
                var links = new List<String>();
                var costs = new List<String>();
                var _salesGameIndex = new List<int>();


                foreach (var node in nodes.Elements())
                {
                    if (node.Name == "a")
                    {
                        names.Add(node.InnerText);
                        links.Add(StoreUrl + node.GetAttributeValue("href", String.Empty));
                    }
                    else if (node.Name == "div")
                    {
                        String cost;

                        var tempCost = node.SelectSingleNode(".//i");
                        if (tempCost != null)
                        {
                            cost = tempCost.InnerText;
                            costs.Add(cost);
                            _salesGameIndex.Add(costs.Count - 1);
                        }
                        else
                        {
                            cost = node.InnerText
                                       .Replace("\t", "")
                                       .Replace("\n", "")
                                       .Replace("\r", "");

                            int ind;
                            if ((ind = cost.IndexOf("&nbsp;")) > -1)
                            {
                                cost = cost.Substring(0, ind);
                                costs.Add(cost);
                            }
                        }
                    }
                }

                if (names.Count == costs.Count && names.Count == links.Count)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        Boolean sale = false;

                        String title = names[i];
                        String searchString = this.DelBadChar(ref title);
                        String gameUrl = links[i];
                        String cost = costs[i];

                        _salesGameIndex.ForEach((item) =>
                        {
                            if (item == i)
                                sale = true;
                        });

                        _entries.Add(new GameEntry()
                        {
                            SearchString = searchString,
                            StoreUrl = StoreUrl,
                            Title = title,
                            GameUrl = gameUrl,
                            Cost = cost,
                            Sale = sale
                        });
                    }
                }
            }
        }

    }
}
