using System.Collections;
using System.Collections.Generic;

public class NetworkMapping
{
    // TODO: read it from config file
    private Dictionary<string, string> nameToIpMapping = new Dictionary<string, string>
    {
        {"TVZ-HOST", "192.168.1.1"},
    };

    public Dictionary<string, string> GetNameToIpMapping()
    {
        return nameToIpMapping;
    }

}
