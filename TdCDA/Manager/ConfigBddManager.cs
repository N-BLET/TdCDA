using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;

namespace TdCDA.Manager
{
    public class ConfigBddManager
{
    private static IFirebaseConfig config = new FirebaseConfig
    {
        AuthSecret = "ZkMCzQKJrloO6GDnEWlx5qcwJjHJaNZ8ZcNZ2yaO",
        BasePath = "https://bddtdcda-default-rtdb.europe-west1.firebasedatabase.app"
    };

    private static IFirebaseClient client;

    public static IFirebaseClient GetClient()
    {
        if (client == null)
        {
            client = new FirebaseClient(config);
        }
        return client;
    }
}
}
