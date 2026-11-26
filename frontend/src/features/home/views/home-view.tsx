import { Link } from '@tanstack/react-router';
import {
  ArrowRight,
  BatteryCharging,
  Cpu,
  Headphones,
  Laptop,
  ShieldCheck,
  Smartphone,
  Sparkles,
  Truck,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { ErrorState, LoadingState } from '@/components/ui/feedback';
import { useGetPublicCategoriesQuery } from '@/shared/api/generated/api.generated';
import { FeaturedProducts } from '../components/featured-products';
import { useFeaturedProducts } from '../hooks/use-featured-products';

const categoryIcons = [Laptop, Smartphone, Headphones, BatteryCharging];

export function HomePage() {
  const featured = useFeaturedProducts();
  const categories = useGetPublicCategoriesQuery();

  return (
    <>
      <section className="page-container pt-7 sm:pt-10" aria-labelledby="hero-title">
        <div className="fancy-panel relative grid min-h-[490px] overflow-hidden rounded-[28px] lg:grid-cols-[1.1fr_.9fr]">
          <div className="relative z-10 flex flex-col items-start justify-center px-8 py-16 sm:px-14 lg:py-20">
            <span className="inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-4 py-2 text-[11px] font-bold tracking-widest text-blue-100">
              <Sparkles size={14} /> A BETTER WAY TO UPGRADE
            </span>
            <h1
              id="hero-title"
              className="mt-7 max-w-xl text-[clamp(2.7rem,5vw,4.7rem)] font-extrabold leading-[1.08] tracking-[-.06em] text-white"
            >
              Power your next <span className="text-[#9ec2ff]">big idea.</span>
            </h1>
            <p className="mt-6 max-w-lg text-base leading-8 text-blue-100/85">
              Discover laptops, audio, mobile tech and accessories — all in one streamlined
              storefront.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Button asChild size="lg" className="bg-white text-blue-800 hover:bg-blue-50">
                <Link to="/shop">
                  Shop electronics <ArrowRight size={17} />
                </Link>
              </Button>
              <Button
                asChild
                size="lg"
                variant="outline"
                className="border-white/30 text-white hover:bg-white/10"
              >
                <Link to="/account">Your account</Link>
              </Button>
            </div>
          </div>
          <div className="relative hidden min-h-full place-items-center lg:grid" aria-hidden="true">
            <div className="absolute size-[360px] rounded-full bg-blue-500/40 blur-[70px]" />
            <div className="relative -rotate-12 rounded-[32px] border border-white/25 bg-gradient-to-br from-slate-300 to-slate-900 p-5 shadow-[30px_35px_50px_#08132b88]">
              <div className="grid h-[245px] w-[330px] place-items-center rounded-2xl bg-gradient-to-tr from-blue-950 via-blue-600 to-cyan-300">
                <Cpu className="size-32 text-white/80" strokeWidth={0.7} />
              </div>
              <div className="mx-auto mt-4 h-1 w-32 rounded-full bg-slate-500" />
            </div>
            <div className="absolute right-14 top-14 rounded-2xl border border-white/25 bg-white/15 px-5 py-4 text-sm font-bold text-white backdrop-blur">
              <Sparkles size={17} className="mb-2" /> Built for what’s next
            </div>
          </div>
        </div>
      </section>
      <section className="page-container mt-9" aria-label="Shopping benefits">
        <div className="grid gap-4 rounded-2xl border border-line bg-white p-5 md:grid-cols-3">
          <p className="flex items-center gap-3 text-sm font-medium">
            <ShieldCheck className="text-moss" size={20} /> Secure customer accounts
          </p>
          <p className="flex items-center gap-3 text-sm font-medium">
            <Cpu className="text-moss" size={20} /> Hardware & accessories
          </p>
          <p className="flex items-center gap-3 text-sm font-medium">
            <Truck className="text-moss" size={20} /> Shipping calculated at checkout
          </p>
        </div>
      </section>
      <section className="page-container py-16" aria-labelledby="categories-title">
        <div>
          <p className="eyebrow">BROWSE YOUR WAY</p>
          <h2 id="categories-title" className="font-display mt-2 text-3xl sm:text-4xl">
            Explore categories
          </h2>
        </div>
        {categories.isLoading ? (
          <LoadingState message="Loading categories…" />
        ) : categories.isError ? (
          <ErrorState error={categories.error} onRetry={categories.refetch} />
        ) : (
          <div className="mt-7 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {categories.data?.map((category, index) => {
              const Icon = categoryIcons[index % categoryIcons.length];
              return (
                <Link
                  key={category.categoryId ?? category.slug ?? index}
                  to="/shop"
                  search={{ q: '', page: 1, sort: 'created-desc', category: category.categoryId }}
                  className="group flex min-h-44 flex-col justify-between rounded-2xl border border-line bg-white p-6 transition-all hover:-translate-y-1 hover:border-blue-300 hover:shadow-lg"
                >
                  <Icon size={32} strokeWidth={1.5} className="text-moss" />
                  <div>
                    <h3 className="text-lg font-bold">{category.name ?? 'Category'}</h3>
                    <p className="mt-1 text-sm text-slate-500">Shop this collection</p>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </section>
      <FeaturedProducts
        items={featured.data?.items ?? []}
        isLoading={featured.isLoading}
        error={featured.error}
        onRetry={() => {
          void featured.refetch();
        }}
      />
    </>
  );
}
