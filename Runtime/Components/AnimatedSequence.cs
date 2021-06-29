using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class AnimatedSequence : AnimatedBehaviour
{
    [System.Flags]
    public enum SequenceTypes
    {
        Image = 1 << 0,
        Sprite = 1 << 1,
    }
    public SequenceTypes types = SequenceTypes.Image;
    public Image image;
    public SpriteRenderer sprite;
    public Sprite[] frameAnimation;
    public bool PlayRaw = false;
    public string RawFileName;
    byte[] rawData = null;
    Texture2D rawTex = null;
    Sprite rawSprite = null;
    int countRawFrame = 0;
    MemoryStream ms = null;
    BinaryReader reader = null;
    int currentRawIndex = -1;

    public AnimatedSequence() : base()
    {
    }
    public AnimatedSequence(Transform _mainTransform) : base(_mainTransform)
    {
    }

    public override void ResetInFromState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (PlayRaw)
        {
            rawData = File.ReadAllBytes(RawFileName);
            countRawFrame = BitConverter.ToInt32(rawData, 0);
            int w = BitConverter.ToInt32(rawData, 4);
            int h = BitConverter.ToInt32(rawData, 8);
            rawTex = new Texture2D(w, h);
            if (image != null || sprite != null) 
                rawSprite = Sprite.Create(rawTex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
            List<byte> list = new List<byte>(rawData);
            list.RemoveRange(0, 12);
            rawData = list.ToArray();
            ms = new MemoryStream(rawData);
            reader = new BinaryReader(ms);
        }
        if (frameAnimation == null || frameAnimation.Length == 0) return;
        if (image != null) image.sprite = frameAnimation[0];
        if (sprite != null) sprite.sprite = frameAnimation[0];
    }

    public override void ResetInToState()
    {
        if (!Used) return;
        t = 0;
        onEnded = false;
        InitTransform();
        if (PlayRaw)
        {
            InitRawData();
        }
        if (frameAnimation == null || frameAnimation.Length == 0) return;
        if (image != null) image.sprite = frameAnimation[frameAnimation.Length - 1];
        if (sprite != null) sprite.sprite = frameAnimation[frameAnimation.Length - 1];
    }

    protected void InitRawData()
    {
        rawData = File.ReadAllBytes(RawFileName);
        countRawFrame = BitConverter.ToInt32(rawData, 0);
        int w = BitConverter.ToInt32(rawData, 4);
        int h = BitConverter.ToInt32(rawData, 8);
        rawTex = new Texture2D(w, h);
        if (image != null || sprite != null)
            rawSprite = Sprite.Create(rawTex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
        List<byte> list = new List<byte>(rawData);
        list.RemoveRange(0, 12);
        rawData = list.ToArray();
        if (reader != null)
            reader.Dispose();
        if (ms != null)
            ms.Dispose();
        ms = new MemoryStream(rawData);
        reader = new BinaryReader(ms);
    }

    protected override object GetFrom()
    {
        return frameAnimation[0];
    }

    protected override object GetTo()
    {
        return frameAnimation[frameAnimation.Length - 1];
    }

    protected override void OnAction(float _time, float curveValue)
    {
        if (Exists(SequenceTypes.Image) && image == null) image = mainTransform.GetComponent<Image>();
        if (Exists(SequenceTypes.Sprite) && sprite == null) sprite = mainTransform.GetComponent<SpriteRenderer>();
        if (image == null && sprite == null) return;
        if ((PlayRaw && rawData == null) || frameAnimation == null || frameAnimation.Length == 0) return;
        if (PlayRaw)
        {
            int num = (int)(_time * (countRawFrame));
            num = Mathf.Clamp(num, 0, countRawFrame - 1);
            if (currentRawIndex != num)
            {
                currentRawIndex = num;
                if (reader.BaseStream.Position + 4 >= reader.BaseStream.Length)
                    reader.BaseStream.Position = 0;
                int len = reader.ReadInt32(); // читаем сколько байт надо взять на кадр
                var bytes = reader.ReadBytes(len);
                rawTex.LoadRawTextureData(bytes);
                rawTex.Apply();
                if (reader.BaseStream.Position >= reader.BaseStream.Length)
                    reader.BaseStream.Position = 0;
            }
            return;
        }
        int n = (int)(_time * (frameAnimation.Length));
        n = Mathf.Clamp(n, 0, frameAnimation.Length - 1);
        if (Exists(SequenceTypes.Image) && image != null) image.sprite = frameAnimation[n];
        if (Exists(SequenceTypes.Sprite) && sprite != null) sprite.sprite = frameAnimation[n];
    }

    protected override void SetFrom(object from)
    {
        throw new NotImplementedException();
    }

    protected override void SetTo(object to)
    {
        throw new NotImplementedException();
    }

    public bool Exists(SequenceTypes type)
    {
        return (types & type) == type;
    }

    public override void Dispose()
    {
        if (reader != null)
            reader.Dispose();
        if (ms != null)
            ms.Dispose();
        reader = null;
        ms = null;
    }
}
