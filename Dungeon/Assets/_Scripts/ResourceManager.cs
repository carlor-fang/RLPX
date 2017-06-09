using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
        #region declaration
        static public ResourceManager instance;
        private Object[] assets;
        // This is used to avoid re-loading the same object from resources in the same frame
        private Dictionary<string, Object> resourcesCache;
        private bool cleaningScheduled;
        #endregion

        #region inherit
        void Awake()
        {
                if (!instance)
                {
                        instance = this;
                }
                else if (instance != this)
                {
                        Destroy(gameObject);
                }

                resourcesCache = new Dictionary<string, Object>();
                cleaningScheduled = false;
        }
        #endregion

        #region private
        private Object FindAsset(string Name)
        {
                if (assets != null)
                {
                        for (int i = 0, imax = assets.Length; i < imax; ++i)
                                if (assets[i] != null && assets[i].name == Name)
                                        return assets[i];
                }
                return null;
        }

        private T LoadFromResources<T>(string Path) where T : Object
        {
                if (string.IsNullOrEmpty(Path))
                        return null;

                Object Obj;
                // Doing Resource.Load is very slow so we are catching the recently loaded objects
                if (resourcesCache.TryGetValue(Path, out Obj) && Obj != null)
                {
                        return Obj as T;
                }

                T obj = null;

                if (Path.EndsWith("]", System.StringComparison.OrdinalIgnoreCase))      // Handle sprites (Multiple) loaded from resources :   "SpritePath[SpriteName]"
                {
                        int idx = Path.LastIndexOf("[", System.StringComparison.OrdinalIgnoreCase);
                        int len = Path.Length - idx - 2;
                        string MultiSpriteName = Path.Substring(idx + 1, len);
                        Path = Path.Substring(0, idx);

                        T[] objs = Resources.LoadAll<T>(Path);
                        for (int j = 0, jmax = objs.Length; j < jmax; ++j)
                                if (objs[j].name.Equals(MultiSpriteName))
                                {
                                        obj = objs[j];
                                        break;
                                }
                }
                else
                        obj = Resources.Load<T>(Path);

                resourcesCache[Path] = obj;

                if (!cleaningScheduled)
                {
                        Invoke("CleanResourceCache", 0.1f);
                        cleaningScheduled = true;
                }
                return obj;
        }
        #endregion

        #region public
        // This function tries finding an asset in the Assets array, if not found it tries loading it from the Resources Folder
        public T GetAsset<T>(string Name) where T : Object
        {
                T Obj = FindAsset(Name) as T;
                if (Obj != null)
                        return Obj;

                return LoadFromResources<T>(Name);
        }

        public void CleanResourceCache()
        {
                resourcesCache.Clear();
                Resources.UnloadUnusedAssets();

                CancelInvoke();
                cleaningScheduled = false;
        }
        #endregion
}
