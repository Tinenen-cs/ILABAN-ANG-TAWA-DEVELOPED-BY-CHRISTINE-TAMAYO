using System;
using UnityEngine;
namespace Hexbound
{
    public enum LanguageMode { Pormal, GenZ }
    [Serializable] public sealed class ReactionCategory { public string category; public string[] lines; }
    [Serializable] public sealed class CharacterReaction { public int id; public string[] lines; }
    [Serializable] public sealed class ReactionData { public string LanguageMode; public ReactionCategory[] Pormal, GenZ; public CharacterReaction[] characters; }
    public sealed class ReactionBank
    {
        public LanguageMode Mode => LanguageMode.Pormal; // Reaction style is no longer a saved/user-selectable setting.
        public readonly ReactionData Data;
        readonly System.Random random=new System.Random(1703);
        readonly string[] last=new string[2];
        readonly float[] next=new float[2],shownAt=new float[2];
        readonly int[] priority=new int[2];
        public ReactionBank() { Data=JsonUtility.FromJson<ReactionData>(Resources.Load<TextAsset>("ReaksyonTeksto").text); }
        public string Choose(int side,int character,string category,float now) {
            int rank=category=="lose"||category=="win"?4:category=="dizzy"?3:category=="hurt"?2:1;
            if(now<next[side] && rank<=priority[side]) return null;
            if(now-shownAt[side]<.09f && priority[side]>0 && rank<3) return null;
            var categories=Data.Pormal;
            string[] lines=null;
            foreach(var entry in categories) if(entry.category==category) lines=entry.lines;
            if(lines==null || lines.Length==0) return null;
            int choice=random.Next(lines.Length);
            if(lines.Length>1 && lines[choice]==last[side]) choice=(choice+1)%lines.Length;
            last[side]=lines[choice]; next[side]=now+.7f;shownAt[side]=now;priority[side]=rank; return last[side];
        }
    }
}
