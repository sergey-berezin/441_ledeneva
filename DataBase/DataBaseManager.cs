using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBase
{
    public class DataBaseManager
    {
        private readonly LibContext _db = new();

        public event Action DataChanged;

        public async Task AddAsync(DBItem item)
        {
            if (SearchForItem(item)) 
                return;

            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();

            DataChanged?.Invoke();
        }

        public void Clear()
        {
            _db.Items.RemoveRange(_db.Items);
            _db.SaveChanges();
            DataChanged?.Invoke();
        }

        bool SearchForItem(DBItem item)
        {
            bool result = false;

            var query = _db.Items.Where(x => (x.X1 == item.X1) && (x.X2 == item.X2) && (x.Y1 == item.Y1) && (x.Y2 == item.Y2));
            foreach (var elem in query)
            {
                if (elem.Img.SequenceEqual(item.Img))
                    result = true;
            }

            return result;
        }

        public IEnumerable<string> GetImgTypes()
        {
            IEnumerable<string> result = _db.Items.Select(x => x.Label).Distinct().Select(x => $"[{_db.Items.Count(y => y.Label == x)}] {x}");
            return result;
        }

        public IEnumerable<byte[]> GetImages(string label)
        {
            IEnumerable<byte[]> result = _db.Items.Where(x => x.Label == label).Select(x => x.Img);
            return result;
        }
    }
}
