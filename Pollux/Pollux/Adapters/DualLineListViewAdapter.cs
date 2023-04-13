using Android.App;
using Android.Views;
using Android.Widget;

namespace Pollux.Adapters
{
    public class DualLineListViewAdapter : BaseAdapter<string>
    {
        string[][] items;
        Activity context;
        public DualLineListViewAdapter(Activity context, string[][] items) : base()
        {
            this.context = context;
            this.items = items;
        }

        public void SetItems(string[][] items)
        {
            if (items != null)
            {
                this.items = items;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override string this[int position]
        {
            get { return items[position][0]; }
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            }
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position][0];
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = items[position][1];
            return view;
        }
    }
}