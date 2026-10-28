using NUnit.Framework;
using UnityEngine;
using TMPro;
namespace Hexbound.Tests
{
    public class RubricTests
    {
        [Test] public void CompleteRubricSuite(){Assert.GreaterOrEqual(RuleChecks.Run(),65);}
        [Test] public void FiftyHitsHaveIdenticalDisplacement(){for(int i=0;i<50;i++){var m=new DuelModel(i%8,(i+1)%8);m.players[0].x=500;m.players[1].x=580;float before=m.players[1].x;m.Tick(.016f,new DuelInput{punch=true},default);for(int k=0;k<30;k++)m.Tick(.016f,default,default);Assert.That(m.players[1].x-before,Is.EqualTo(DuelModel.StepBack).Within(.0001f));}}
        [Test] public void EveryStateRejectsForbiddenMoves(){foreach(Pose pose in new[]{Pose.Idle,Pose.Walk,Pose.Punch,Pose.Hurt,Pose.Dizzy}){var m=new DuelModel(0,1);float initialX=m.players[0].x;m.players[0].pose=pose;if(pose==Pose.Hurt)m.players[0].stun=.2f;if(pose==Pose.Dizzy)m.players[0].dizzyTime=1;for(int i=0;i<30;i++)m.Tick(.016f,new DuelInput{jump=true,push=true,special=true,power=true,block=true},default);Assert.AreEqual(initialX,m.players[0].x);Assert.That(m.players[0].pose,Is.Not.EqualTo(Pose.Jump));}}
        [Test] public void ReactionsAreFormalAndDoNotTouchModel(){var bank=new ReactionBank();Assert.AreEqual(LanguageMode.Pormal,bank.Mode);var a=new DuelModel(0,1);var b=new DuelModel(0,1);for(int i=0;i<180;i++){bank.Choose(0,0,"taunt",i*.016f);a.Tick(.016f,new DuelInput{move=1,punch=i==0},default);b.Tick(.016f,new DuelInput{move=1,punch=i==0},default);}Assert.AreEqual(a.players[0].x,b.players[0].x);Assert.AreEqual(a.players[1].x,b.players[1].x);Assert.AreEqual(a.players[0].actionTime,b.players[0].actionTime);}
        [Test] public void ReactionCooldownAndNoRepeat(){var bank=new ReactionBank();string first=bank.Choose(0,0,"taunt",1);Assert.IsNotNull(first);Assert.IsNull(bank.Choose(0,0,"taunt",1.1f));Assert.AreNotEqual(first,bank.Choose(0,0,"taunt",2));}
        [Test] public void StaticFontContainsRequiredCharacters(){var font=Resources.Load<TMP_FontAsset>("Ilaban/Fonts/KalyeBody");Assert.IsNotNull(font);Assert.AreEqual(AtlasPopulationMode.Static,font.atlasPopulationMode);foreach(char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzÑñ'-.,!?:[]0123456789")Assert.IsTrue(font.HasCharacter(c),"Missing "+c);Assert.AreEqual(FilterMode.Bilinear,font.atlasTexture.filterMode);}
        [Test] public void ThreeMapsUseTheirApprovedFixedLight(){Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Maps/Palengke/Palengke_Umaga"));foreach(var map in new[]{"Looban","Plaza"})Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Maps/"+map+"/"+map+"_Gabi"));}
        [Test] public void UnusedMapTimesAreOutsideResources(){foreach(var n in new[]{"Palengke_Hapon","Palengke_Takipsilim","Looban_Hapon","Looban_Umaga","Plaza_Hapon","Plaza_Umaga"}){string map=n.Substring(0,n.IndexOf('_'));Assert.IsNull(Resources.Load<Texture2D>("Ilaban/Maps/"+map+"/"+n));}}
        [Test] public void NewOpeningAndCanalArtAreImported(){Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/UI/JeepneyFrames"));Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Maps/FX/Kanal"));}
        [Test] public void EveryOriginalAdultRigIsComplete(){foreach(var key in Roster.Files)foreach(var part in new[]{"Body","Portrait","Head","Torso","ArmL","ArmR","LegL","LegR"})Assert.IsNotNull(Resources.Load<Texture2D>("Ilaban/Characters/"+key+"/"+part));}
        [Test] public void EveryMapHasDifferentMusic(){var clips=new AudioClip[3];for(int i=0;i<3;i++){clips[i]=Resources.Load<AudioClip>("Ilaban/Audio/Music/"+Roster.StageFiles[i]);Assert.IsNotNull(clips[i]);for(int k=0;k<i;k++)Assert.AreNotSame(clips[i],clips[k]);}}
        [Test] public void MatchRequiresThreeDrainWinsAndKeepsTallyBetweenRounds(){
            for(int side=0;side<2;side++){var m=new DuelModel(0,7);Assert.Less(m.players[1].x-m.players[0].x-DuelModel.PunchReach,60,"Only a short step is needed.");
                for(int win=1;win<=3;win++){m.players[1-side].x=side==0?1200:80;m.Tick(.016f,default,default);Assert.IsTrue(m.Finished);Assert.AreEqual(win,m.players[side].wins);Assert.AreEqual(win==3,m.MatchFinished);Assert.AreEqual(win<3,m.BeginNextRound());if(win<3){Assert.IsFalse(m.Finished);Assert.AreEqual(win+1,m.round);}}
            }
        }
        [Test] public void SharedWeaponReachHasOneBoundaryForAllCharacters(){
            for(int id=0;id<8;id++)foreach(float delta in new[]{-.5f,.5f}){var m=new DuelModel(id,(id+1)%8);m.players[0].x=500;m.players[1].x=500+DuelModel.PunchReach+delta;float before=m.players[1].x;m.Tick(.016f,new DuelInput{punch=true},default);for(int f=0;f<12;f++)m.Tick(.016f,default,default);Assert.AreEqual(delta<0?DuelModel.StepBack:0,m.players[1].x-before,.001f);}
        }
    }
}
