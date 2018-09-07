using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DReAM.Models;

namespace DReAM.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Action="New", Text = "New", Description="Create a new Juliar DReAM Program" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Open", Text = "Open", Description="Open existing Juliar DReAM Program" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Interpret", Text = "Interpret", Description="Interpret your commands live" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Chat", Text = "Chat", Description="Chat with other DReAM users" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Docs", Text = "Docs", Description="Get Documentation" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Share", Text = "Share", Description="Tell your friends about the program" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Contact", Text = "Contact", Description="Contact the development team" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Help", Text = "Help out", Description="Help with development of the project" },
                new Item { Id = Guid.NewGuid().ToString(), Action="Settings", Text = "Settings", Description="Settings of the application" },
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}