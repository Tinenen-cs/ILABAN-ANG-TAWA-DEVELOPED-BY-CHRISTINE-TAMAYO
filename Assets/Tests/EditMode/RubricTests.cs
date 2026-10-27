using NUnit.Framework;
using UnityEngine;
using TMPro;
namespace Hexbound.Tests
{
    public class RubricTests
    {
        [Test] public void CompleteRubricSuite(){Assert.GreaterOrEqual(RuleChecks.Run(),65);}
        [Test] public void FiftyHitsHaveIdenticalDisplacement(){for(int i=0;i<50;i++){var m=new DuelModel(i%8,(i+1)%8);m.players[0].x=500;m.players[1].x=580;float before=m.players[1].x;m.Tick(.016f,new DuelInput{punch=true},default);for(int k=0;k<30;k++)m.Tick(.016f,default,default);Assert.That(m.players[1].x-before,Is.EqualTo(DuelModel.StepBack).Within(.0001f));}}
        [Test] public void EveryStateRejectsForbiddenMoves(){foreach(Pose pose in new[]{Pose.Idle,Pose.Walk,Pose.Punch,Pose.Hurt,Pose.Dizzy}){var m=new DuelModel(0,1);m.players[0].pose=pose;if(pose==Pose.Hurt)m.players[0].stun=.2f;if(pose==Pose.Dizzy)m.players[0].dizzyTime=1;for(int i=0;i<30;i++)m.Tick(.016f,new DuelInput{jump=true,push=true,special=true,power=true,block=true},default);Assert.AreEqual(430,m.players[0].x);Assert.That(m.players[0].pose,Is.Not.EqualTo(Pose.Jump));}}
        [Test] public void ReactionsAreFormalAndDoNotTouchModel(){var bank=new ReactionBank();Assert.AreEqual(LanguageMode.Pormal,bank.Mode);var a=new DuelModel(0,1);var b=new DuelModel(0,1);for(int i=0;i<180;i++){bank.Choose(0,0,"taunt",i*.016f);a.Tick(.016f,new DuelInput{move=1,punch=i==0},default);b.Tick(.016f,new DuelInput{move=1,punch=i==0},default);}Assert.AreEqual(a.players[0].x,b.players[0].x);Assert.AreEqual(a.players[1].x,b.players[1].x);Assert.AreEqual(a.players[0].actionTime,b.players[0].actionTime);}
        [Test] public void ReactionCooldownAndNoRepeat(){var bank=new ReactionBank();string first=bank.Choose(0,0,"taunt",1);Assert.IsNotNull(first);Assert.IsNull(bank.Choose(0,0,"taunt",1.1f));Assert.AreNotEqual(first,bank.Choose(0,0,"taunt",2));}
        [Test] public void StaticFontContainsRequiredCharacters(){var font=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeBody");Assert.IsNotNull(font);Assert.AreEqual(AtlasPopulationMode.Static,font.atlasPopulationMode);foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÑñ'-.,!?:[]0123456789")Assert.IsTrue(font.HasCharacter(c),"Missing "+c);Assert.AreEqual(FilterMode.Bilinear,font.atlasTexture.filterMode);}
        [Test] public void EveryTimeOfDayHasImportedMap(){foreach(var map in Roster.StageFiles)foreach(var time in new[]{"Umaga","Hapon","Takipsilim"})Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Maps/"+map+"/"+map+"_"+time));}
        [Test] public void EveryOriginalAdultRigIsComplete(){foreach(var key in Roster.Files)foreach(var part in new[]{"Body","Portrait","Head","Torso","ArmL","ArmR","LegL","LegR"})Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Characters/"+key+"/"+part));}
        [Test] public void EveryMapHasDifferentMusic(){var clips=new AudioClip[3];for(int i=0;i<3;i++){clips[i]=Resources.Load<AudioClip>("Ilaban/Audio/Music/"+Roster.StageFiles[i]);Assert.IsNotNull(clips[i]);for(int k=0;k<i;k++)Assert.AreNotSame(clips[i],clips[k]);}}
    }
}
