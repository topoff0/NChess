import { Link } from "react-router-dom";

type StartButtonProps = {
  to: string;
  text: string;
};

export const StartButton = ({to, text} : StartButtonProps) => (
  <div>
    <Link to={to}
      className="group relative inline-flex -rotate-1 items-center justify-center gap-3 overflow-hidden rounded-2xl border-3 border-primary bg-accent-200 px-10 py-4 text-lg font-black uppercase tracking-[0.14em] text-primary-dark shadow-under-10 transition duration-300 transform-gpu hover:-translate-y-1 hover:bg-primary hover:text-primary-dark focus-visible:outline-none focus-visible:ring-4 focus-visible:ring-accent-100/40">
      <span className="absolute inset-0 translate-y-full bg-primary/20 transition duration-300 group-hover:translate-y-0" />
      <span className="relative">{text}</span>
      <span className="relative text-2xl transition group-hover:translate-x-1 group-hover:-translate-y-1">♜</span>
    </Link>
  </div>
)
