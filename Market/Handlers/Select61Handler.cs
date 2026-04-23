namespace tser
{
    internal class Select61Handler
    {
        private InputSimulator sim;
        private Mover mover;

        public Select61Handler(InputSimulator sim)
        {
            this.sim = sim;
            mover = new Mover(sim);
        }

        public async Task Run(HandlerContext context)
        {
            await mover.MoveAndClick(457, 393, 579, 403);

            await mover.MoveAndClick(460, 471, 576, 488);


            await mover.MoveAndClick(614, 392, 727, 405);

            await mover.MoveAndClick(612, 442, 732, 459);


            await mover.MoveAndClick(768, 394, 884, 405);
            await mover.MoveAndClick(764, 445, 880, 458);
        }
    }
}
