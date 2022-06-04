using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public NftCard card;

    public Text nameText;
    public Text decriptionText;
    public Text contractText;

    public Image imageImage;

    public Text startBidText;


    void Start()
    {
        nameText.text = card.nftname;
        decriptionText.text = card.description;
        contractText.text = card.contract;
        imageImage.sprite = card.image;
        startBidText.text = card.startingBid.ToString();

    }

    
}
