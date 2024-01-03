using DresserMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGeneralsTraining
{
    public static class Dresser
    {
        public static void Intitialize()
        {
            StorageRoom.AddSubscriber(ResourcesController.AssetsFolder);
        }

        public static void AddToWardrobe(Attire attire)
        {
            if (!StorageRoom.wardrobes.ContainsKey(attire.wearer))
            {
                StorageRoom.wardrobes.Add(attire.wearer, new Wardrobe(attire.wearer));
            }
            StorageRoom.wardrobes[attire.wearer].AddAttire(attire);
        }
    }
}
