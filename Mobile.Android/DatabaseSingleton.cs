// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using DrunkAudible.Data;

namespace DrunkAudible.Mobile.Android
{
    public static class DatabaseSingleton
    {
        static ObjectRelationalMapping _orm;

        public static ObjectRelationalMapping Orm
        {
            get
            {
                if (_orm == null)
                {
                    _orm = new ObjectRelationalMapping ();
                }
                return _orm;
            }
        }
    }
}

