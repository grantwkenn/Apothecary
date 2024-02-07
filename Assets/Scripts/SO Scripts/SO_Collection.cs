using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO Collection", menuName = "SO Collection")]
public class SO_Collection : ScriptableObject
{
    [SerializeField]
    List<ScriptableObject> collection;

    public List<ScriptableObject> getCollection() { return this.collection; }
}
