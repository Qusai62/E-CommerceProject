import { Link } from '@tanstack/react-router';
import { useState, type FormEvent } from 'react';
import { Building2, Plus, Search } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { EmptyState, ErrorState, Field, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { useCreateSupplierMutation, useGetSuppliersQuery } from '@/shared/api/suppliers-api';
import { date } from '@/shared/lib/format';

export function OwnerSuppliersPage() {
  const [name, setName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [email, setEmail] = useState('');
  const [searchDraft, setSearchDraft] = useState('');
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState('name');
  const [sortDescending, setSortDescending] = useState(false);
  const [createdSupplier, setCreatedSupplier] = useState<{
    supplierId: number;
    name: string;
    createdAtUtc: string;
  }>();
  const [createSupplier, status] = useCreateSupplierMutation();
  const suppliers = useGetSuppliersQuery({
    Search: search || undefined,
    PageNumber: page,
    PageSize: 10,
    SortBy: sortBy,
    SortDescending: sortDescending,
  });

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const supplierName = name.trim();
    if (!supplierName || status.isLoading) return;
    try {
      const supplier = await createSupplier({
        name: supplierName,
        phoneNumber: phoneNumber.trim() || null,
        email: email.trim() || null,
      }).unwrap();
      setCreatedSupplier(supplier);
      setName('');
      setPhoneNumber('');
      setEmail('');
    } catch {
      // The API error is rendered below the form.
    }
  };

  const applySearch = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSearch(searchDraft.trim());
    setPage(1);
  };

  return (
    <div className="max-w-4xl space-y-7">
      <PageHeader
        eyebrow="PROCUREMENT"
        title="Suppliers"
        description="Create the supplier records that will be used for purchase invoices and supplier payments."
      />
      {createdSupplier && (
        <Card className="border-emerald-200 bg-emerald-50 p-5 text-emerald-950">
          <p role="status" className="font-semibold">
            Supplier created
          </p>
          <p className="mt-1 text-sm">
            {createdSupplier.name} was saved with supplier ID #{createdSupplier.supplierId} on{' '}
            {date(createdSupplier.createdAtUtc)}.
          </p>
          <Button asChild size="sm" variant="outline" className="mt-4 border-emerald-300 bg-white">
            <Link
              to="/owner/suppliers/$supplierId"
              params={{ supplierId: String(createdSupplier.supplierId) }}
            >
              View supplier
            </Link>
          </Button>
        </Card>
      )}
      <Card className="p-6 sm:p-8">
        <div className="flex items-start gap-3">
          <span className="grid size-11 place-items-center rounded-xl bg-blue-100 text-blue-700">
            <Building2 size={22} />
          </span>
          <div>
            <h2 className="text-xl font-bold">New supplier</h2>
            <p className="mt-1 text-sm text-slate-500">
              A supplier name is required. Contact details are optional.
            </p>
          </div>
        </div>
        <form
          className="mt-7 grid gap-5 sm:grid-cols-2"
          noValidate
          onSubmit={(event) => void submit(event)}
        >
          <div className="sm:col-span-2">
            <Field label="Supplier name">
              <Input
                value={name}
                onChange={(event) => setName(event.target.value)}
                placeholder="e.g. Amman Electronics Distribution"
                required
                maxLength={200}
                autoComplete="organization"
              />
            </Field>
          </div>
          <Field label="Phone number (optional)">
            <Input
              value={phoneNumber}
              onChange={(event) => setPhoneNumber(event.target.value)}
              placeholder="e.g. 06 555 0000"
              type="tel"
              autoComplete="tel"
            />
          </Field>
          <Field label="Email (optional)">
            <Input
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="accounts@example.com"
              type="email"
              autoComplete="email"
            />
          </Field>
          <div className="sm:col-span-2 flex flex-wrap items-center gap-4 border-t border-slate-200 pt-5">
            <Button type="submit" disabled={status.isLoading || !name.trim()}>
              {status.isLoading ? (
                'Creating supplier…'
              ) : (
                <>
                  <Plus size={16} /> Create supplier
                </>
              )}
            </Button>
            <FormError error={status.error} />
          </div>
        </form>
      </Card>
      <Card className="p-6 sm:p-8">
        <div className="flex items-center gap-3">
          <Building2 size={23} className="text-blue-600" />
          <div>
            <h2 className="text-xl font-bold">Suppliers</h2>
            <p className="mt-1 text-sm text-slate-500">Search and open your supplier records.</p>
          </div>
        </div>
        <form
          className="mt-6 grid gap-3 sm:grid-cols-[minmax(0,1fr)_160px_auto]"
          onSubmit={applySearch}
        >
          <div className="relative">
            <Input
              value={searchDraft}
              onChange={(event) => setSearchDraft(event.target.value)}
              placeholder="Search name, email, or phone"
              className="pr-10"
            />
            <Search
              size={18}
              className="pointer-events-none absolute right-3 top-3 text-slate-400"
            />
          </div>
          <select
            aria-label="Supplier sort order"
            value={`${sortBy}:${sortDescending}`}
            onChange={(event) => {
              const [field, descending] = event.target.value.split(':');
              setSortBy(field);
              setSortDescending(descending === 'true');
              setPage(1);
            }}
            className="h-11 rounded-xl border border-line bg-white px-3 text-sm"
          >
            <option value="name:false">Name A–Z</option>
            <option value="name:true">Name Z–A</option>
            <option value="createdAtUtc:true">Newest first</option>
            <option value="createdAtUtc:false">Oldest first</option>
          </select>
          <Button type="submit" variant="secondary">
            Search
          </Button>
        </form>
        {suppliers.isLoading ? (
          <LoadingState message="Loading suppliers…" />
        ) : suppliers.isError ? (
          <ErrorState error={suppliers.error} onRetry={suppliers.refetch} />
        ) : suppliers.data?.items.length ? (
          <>
            <div className="mt-6 overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="border-b border-line text-xs uppercase tracking-wide text-stone-500">
                  <tr>
                    <th className="pb-3 pr-4">Supplier</th>
                    <th className="pb-3 pr-4">Contact</th>
                    <th className="pb-3 pr-4">Created</th>
                    <th className="pb-3">
                      <span className="sr-only">Actions</span>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {suppliers.data.items.map((supplier) => (
                    <tr key={supplier.supplierId} className="border-b border-line last:border-0">
                      <td className="py-4 pr-4">
                        <p className="font-semibold text-ink">{supplier.name}</p>
                        <p className="mt-1 text-xs text-slate-400">#{supplier.supplierId}</p>
                      </td>
                      <td className="py-4 pr-4 text-slate-600">
                        {supplier.email || supplier.phoneNumber || '—'}
                      </td>
                      <td className="py-4 pr-4 text-slate-600">{date(supplier.createdAtUtc)}</td>
                      <td className="py-4 text-right">
                        <Button asChild size="sm" variant="outline">
                          <Link
                            to="/owner/suppliers/$supplierId"
                            params={{ supplierId: String(supplier.supplierId) }}
                          >
                            View
                          </Link>
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="mt-6 flex items-center justify-between gap-3">
              <p className="text-sm text-slate-500">
                Page {suppliers.data.pageNumber} of {Math.max(1, suppliers.data.totalPages)} ·{' '}
                {suppliers.data.totalCount} suppliers
              </p>
              <div className="flex gap-2">
                <Button
                  size="sm"
                  variant="secondary"
                  disabled={page <= 1}
                  onClick={() => setPage((current) => current - 1)}
                >
                  Previous
                </Button>
                <Button
                  size="sm"
                  variant="secondary"
                  disabled={page >= suppliers.data.totalPages}
                  onClick={() => setPage((current) => current + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          </>
        ) : (
          <EmptyState
            icon={<Building2 size={30} />}
            title="No suppliers found"
            description={
              search
                ? 'Try another search or create a supplier.'
                : 'Create your first supplier above.'
            }
          />
        )}
      </Card>
      <p className="text-xs leading-5 text-slate-500">
        This screen uses <code>GET /api/suppliers</code> and <code>POST /api/suppliers</code>.
      </p>
    </div>
  );
}
