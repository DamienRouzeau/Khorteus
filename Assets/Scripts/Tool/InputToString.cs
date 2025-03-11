using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputToString : MonoBehaviour
{
    private Dictionary<Key, Sprite> keyImages = new Dictionary<Key, Sprite>();
    public Sprite defaultSprite, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z, ctrl, shift, alt, tab, space, suppr, enter, escape, one, two, tree, four, five, six, seven, eight, nine, zero, LMB, RMB, MouseMiddle, MouseBack, MouseNext;
    public Dictionary<MouseButton, Sprite> mouseButtonImages = new Dictionary<MouseButton, Sprite>();

    private void Start()
    {
        keyImages[Key.A] = a;
        keyImages[Key.B] = b;
        keyImages[Key.C] = c;
        keyImages[Key.D] = d;
        keyImages[Key.E] = e;
        keyImages[Key.F] = f;
        keyImages[Key.G] = g;
        keyImages[Key.H] = h;
        keyImages[Key.I] = i;
        keyImages[Key.J] = j;
        keyImages[Key.K] = k;
        keyImages[Key.L] = l;
        keyImages[Key.M] = m;
        keyImages[Key.N] = n;
        keyImages[Key.O] = o;
        keyImages[Key.P] = p;
        keyImages[Key.Q] = q;
        keyImages[Key.R] = r;
        keyImages[Key.S] = s;
        keyImages[Key.T] = t;
        keyImages[Key.U] = u;
        keyImages[Key.V] = v;
        keyImages[Key.W] = w;
        keyImages[Key.X] = x;
        keyImages[Key.Y] = y;
        keyImages[Key.Z] = z;
        keyImages[Key.LeftCtrl] = ctrl;
        keyImages[Key.RightCtrl] = ctrl;
        keyImages[Key.LeftShift] = shift;
        keyImages[Key.RightShift] = shift;
        keyImages[Key.Tab] = tab;
        keyImages[Key.Space] = space;
        keyImages[Key.Enter] = enter;
        keyImages[Key.Escape] = escape;
        keyImages[Key.Numpad1] = one;
        keyImages[Key.Digit1] = one;
        keyImages[Key.Numpad2] = two;
        keyImages[Key.Digit2] = two;
        keyImages[Key.Numpad3] = tree;
        keyImages[Key.Digit3] = tree;
        keyImages[Key.Numpad4] = four;
        keyImages[Key.Digit4] = four;
        keyImages[Key.Numpad5] = five;
        keyImages[Key.Digit5] = five;
        keyImages[Key.Numpad6] = six;
        keyImages[Key.Digit6] = six;
        keyImages[Key.Numpad7] = seven;
        keyImages[Key.Digit7] = seven;
        keyImages[Key.Numpad8] = eight;
        keyImages[Key.Digit8] = eight;
        keyImages[Key.Numpad9] = nine;
        keyImages[Key.Digit9] = nine;
        keyImages[Key.Numpad0] = zero;
        keyImages[Key.Digit0] = zero;
        keyImages[Key.AltGr] = alt;
        keyImages[Key.LeftAlt] = alt;
        keyImages[Key.RightAlt] = alt;
        mouseButtonImages[MouseButton.Right] = RMB;
        mouseButtonImages[MouseButton.Left] = LMB;
        mouseButtonImages[MouseButton.Middle] = MouseMiddle;
        mouseButtonImages[MouseButton.Back] = MouseBack;
        mouseButtonImages[MouseButton.Forward] = MouseNext;


    }

    public Sprite InputToImage(Key key)
    {
        if (keyImages.TryGetValue(key, out Sprite sprite))
        {
            return sprite;
        }
        return defaultSprite; 
    }

    public Sprite InputToImage(MouseButton button)
    {
        if (mouseButtonImages.TryGetValue(button, out Sprite sprite))
        {
            return sprite;
        }
        return defaultSprite;
    }

    public string GetAllKeys()
    {
        StringBuilder keyList = new StringBuilder();

        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            keyList.Append(key.ToString()).Append(", ");
        }

        return keyList.ToString().TrimEnd(',', ' ');
    }

    public string GetAllMouseButtons()
    {
        StringBuilder buttonList = new StringBuilder();

        foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
        {
            buttonList.Append(button.ToString()).Append(", ");
        }

        return buttonList.ToString().TrimEnd(',', ' ');
    }
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    Back,
    Forward
}

