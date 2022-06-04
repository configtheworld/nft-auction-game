using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NFT", menuName ="NFTCard")]
public class NftCard : ScriptableObject
{
    public string nftname;
    public string description;
    public string contract;

    public Sprite image;

    public int startingBid;
}
