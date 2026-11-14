namespace HollowSeven {
public readonly struct StoryShot {
    public readonly string Id, Line, Place, Speaker;
    public readonly float Seconds;
    public StoryShot(string id,string line,string place,string speaker,float seconds=8){Id=id;Line=line;Place=place;Speaker=speaker;Seconds=seconds;}
}
public static class StoryScript {
    public static readonly StoryShot[] Opening={
        new StoryShot("PH","This was our home. Three weeks ago, my wife packed a suitcase and told me our marriage was over.","THREE WEEKS AGO  /  YOUR HOME","HERO · YOUR CHARACTER"),
        new StoryShot("PH","I can't keep pretending. I've fallen in love with someone else. I'm going to live with him.","THREE WEEKS AGO  /  YOUR HOME","YOUR WIFE"),
        new StoryShot("P3","I asked her to stay. She left anyway. I kept wearing the ring, as if that could change her mind.","AFTER SHE LEFT","HERO"),
        new StoryShot("P2","This afternoon I saw her again. The man with the flowers was her new boyfriend—the man she had left me for.","TODAY, AFTERNOON  /  OUTSIDE A CAFÉ","HERO"),
        new StoryShot("P2","They looked happy. I couldn't bring myself to speak to her. I stood across the street until they were gone.","TODAY, AFTERNOON  /  OUTSIDE A CAFÉ","HERO"),
        new StoryShot("P3","By night, I'd been drinking. I didn't want to go back to the empty house. I wanted somewhere I could stop thinking about them.","TONIGHT  /  WALKING ALONE","HERO"),
        new StoryShot("P0","I stopped in the alley behind a casino: Cursed Table. I was still deciding whether to go in when a voice came through the door.","TONIGHT  /  BEHIND THE CASINO","HERO"),
        new StoryShot("P0","Still wearing your wedding ring? Come inside. There are things about her new life you haven't seen.","TONIGHT  /  THE CASINO'S REAR ENTRANCE","A VOICE BEHIND THE DOOR"),
        new StoryShot("P4","The door opened before I touched it. I should have walked away. Instead, I followed the voice to a roulette table.","MOMENTS LATER  /  ENTERING CURSED TABLE","HERO"),
        new StoryShot("P8","Money isn't what you want. You want to know whether leaving you made her happy. My table can show you.","AT THE ROULETTE TABLE","THE DEALER"),
        new StoryShot("P8","Combine the numbered chips to reach my target. Lucky numbers fill the meter; cursed numbers drain it. Reach +15 and I'll reveal a memory.","AT THE ROULETTE TABLE","THE DEALER",10),
        new StoryShot("P8","A memory of her? Then deal. I want to see what happened after she walked away from me.","THE FIRST HAND","HERO")
    };
    public static readonly StoryShot[] FirstMemory={
        new StoryShot("F1","The casino faded. I was looking into her new apartment. This had happened an hour before I sat at the table.","ONE HOUR EARLIER  /  HER NEW APARTMENT","HERO · WATCHING A MEMORY"),
        new StoryShot("F1","Why is your bag packed? You said we were going to build a life together. I left my husband because I believed you.","ONE HOUR EARLIER  /  HER NEW APARTMENT","YOUR WIFE"),
        new StoryShot("F1","I know what I said. I've changed my mind. I'm leaving tonight.","ONE HOUR EARLIER  /  HER NEW APARTMENT","HER BOYFRIEND"),
        new StoryShot("F2","She reached for him. He picked up his bag and walked past her. The man she chose had decided to leave her too.","THE BREAKUP  /  HE LEAVES","HERO · WATCHING A MEMORY"),
        new StoryShot("F3","She was alone. She opened my contact on her phone, but she didn't call. I had seen none of this from the street.","AFTER HE LEFT  /  HER NEW APARTMENT","HERO · WATCHING A MEMORY"),
        new StoryShot("HS","The table came back into focus. I should have felt sorry for her. Instead, I smiled. For a moment, her pain felt like my reward.","BACK IN THE CASINO  /  THE HERO'S REACTION","HERO"),
        new StoryShot("HL","LUCK... ahh... NUMBERS.. haha... muhahahahahahah","BACK IN THE CASINO  /  THE HERO'S REACTION","HERO",9),
        new StoryShot("FB","Enjoying what you saw? Remember: the table reveals what happened. It doesn't undo it. And every glimpse has a price.","BACK AT THE TABLE","THE DEALER",8)
    };
}
}
