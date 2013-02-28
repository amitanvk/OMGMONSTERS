using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ErMyGerdMernsters
{
    public interface GameElement
    {
        void Update(GameTime gt);
        void Reset();
    }

    public abstract class GameElementCollection<T> : GameElement where T : GameElement
    {
        private List<T> list;

        public GameElementCollection()
        {
            list = new List<T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public T this[int index]
        {
            get 
            {
                if (index >= 0 && index < list.Count)
                {
                    return list[index];
                }
                else
                {
                    throw new System.IndexOutOfRangeException();
                }
            }
        }

        public void Add(T element)
        {
            if (!list.Contains(element))
                list.Add(element);
        }

        public void Remove(T element)
        {
            list.Remove(element);
        }

        public void Clear()
        {
            list.Clear();
        }

        public virtual void Update(GameTime gt)
        {
            for (int i = 0; i < Count; i++)
                this[i].Update(gt);
        }

        public virtual void Reset()
        {
            for (int i = 0; i < Count; i++)
                this[i].Reset();
        }
    }
}
