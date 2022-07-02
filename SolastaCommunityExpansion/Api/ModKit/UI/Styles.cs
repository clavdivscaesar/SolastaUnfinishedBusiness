﻿using UnityEngine;
using UnityModManagerNet;

namespace ModKit;

public static partial class UI
{
    private static Texture2D fillTexture;

    private static readonly Color fillColor = new(1f, 1f, 1f, 0.65f);

    private static GUIStyle _textBoxStyle;

    private static GUIStyle _toggleStyle;

    public static GUIStyle divStyle;

    public static GUIStyle textBoxStyle
    {
        get
        {
            if (_textBoxStyle == null)
            {
                _textBoxStyle = new GUIStyle(GUI.skin.box) {richText = true};
            }

            _textBoxStyle.fontSize = 14.point();
            _textBoxStyle.fixedHeight = 19.point();
            _textBoxStyle.margin = new RectOffset(2.point(), 2.point(), 1.point(), 2.point());
            _textBoxStyle.padding = new RectOffset(2.point(), 2.point(), 0.point(), 0);
            _textBoxStyle.contentOffset = new Vector2(0, 2.point());
#pragma warning disable CS0618 // Type or member is obsolete
            _textBoxStyle.clipOffset = new Vector2(0, 2.point());
#pragma warning restore CS0618 // Type or member is obsolete

            return _textBoxStyle;
        }
    }

    public static GUIStyle toggleStyle
    {
        get
        {
            if (_toggleStyle == null)
            {
                _toggleStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleLeft};
            }

            return _toggleStyle;
        }
    }

    private static int point(this int x)
    {
        return UnityModManager.UI.Scale(x);
    }

    public static void Div(Color color, float indent = 0, float height = 0, float width = 0)
    {
        if (fillTexture == null)
        {
            fillTexture = new Texture2D(1, 1);
        }

        divStyle = new GUIStyle {fixedHeight = 1};
        fillTexture.SetPixel(0, 0, color);
        fillTexture.Apply();
        divStyle.normal.background = fillTexture;
        if (divStyle.margin == null)
        {
            divStyle.margin = new RectOffset((int)indent, 0, 4, 4);
        }
        else
        {
            divStyle.margin.left = (int)indent + 3;
        }

        if (width > 0)
        {
            divStyle.fixedWidth = width;
        }
        else
        {
            divStyle.fixedWidth = 0;
        }

        Space(2f * height / 3f);
        GUILayout.Box(GUIContent.none, divStyle);
        Space(height / 3f);
    }
}
