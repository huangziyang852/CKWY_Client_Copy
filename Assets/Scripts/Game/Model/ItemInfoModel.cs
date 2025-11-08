using LaunchPB;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Game.Model
{
    public class ItemInfoModel
    {
        public List<ItemModel> Items { get; set; } = new ();

        public ItemModel GetItem(int id)
        {
            return Items.FirstOrDefault(i => i.ItemId == id);
        }

        /// <summary>
        /// 添加或更新道具数量（增量）
        /// </summary>
        public void AddOrUpdateItem(int itemId, int count)
        {
            var item = Items.FirstOrDefault(i => i.ItemId == itemId);

            if (item != null)
            {
                item.Count.Value += count;
            }
            else
            {
                var newItem = new ItemModel(itemId, count);
                Items.Add(newItem);
            }
        }
    }

    public class ItemModel
    {
        public int ItemId { get; set; }

        public ReactiveProperty<int> Count = new ReactiveProperty<int>(0);

        public ItemModel(int itemId, int count)
        {
            ItemId = itemId;
            Count = new ReactiveProperty<int>(count);
        }

    }
}
