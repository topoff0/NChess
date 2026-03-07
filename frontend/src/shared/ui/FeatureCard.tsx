type FeatureCardProps = {
  title: string;
  subtitle: string;
  description: string;
  colorClass: string;
};

export const FeatureCard = ({ title, subtitle, description, colorClass }: FeatureCardProps) => (
  <div className={`${colorClass} rounded-3xl border-2 p-5 shadow-under-10 text-left backdrop-blur-sm transition hover:-translate-y-1`}>
    <p className="text-xs uppercase font-black tracking-widest text-accent-100">{title}</p>
    <p className="mt-2 text-2xl font-black text-primary">{subtitle}</p>
    <p className="mt-2 text-sm text-primary/85">{description}</p>
  </div>
)
