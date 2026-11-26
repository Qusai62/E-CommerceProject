import { Link, getRouteApi } from '@tanstack/react-router';
import { useEffect, useState, type FormEvent } from 'react';
import { ArrowLeft, Building2, Mail, Phone } from 'lucide-react';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ErrorState, Field, FormError, LoadingState } from '@/components/ui/feedback';
import { Input } from '@/components/ui/input';
import { useGetSupplierByIdQuery, useUpdateSupplierMutation } from '@/shared/api/suppliers-api';
import { date } from '@/shared/lib/format';

const route = getRouteApi('/owner/suppliers_/$supplierId');

export function OwnerSupplierDetailsPage() {
  const { supplierId } = route.useParams();
  const id = Number(supplierId);
  const supplier = useGetSupplierByIdQuery({ id }, { skip: !Number.isSafeInteger(id) || id < 1 });
  const [name, setName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [email, setEmail] = useState('');
  const [saved, setSaved] = useState(false);
  const [updateSupplier, updateStatus] = useUpdateSupplierMutation();

  useEffect(() => {
    if (!supplier.data) return;
    setName(supplier.data.name);
    setPhoneNumber(supplier.data.phoneNumber ?? '');
    setEmail(supplier.data.email ?? '');
  }, [supplier.data]);

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const supplierName = name.trim();
    if (!supplierName || updateStatus.isLoading) return;
    try {
      await updateSupplier({
        id,
        body: {
          name: supplierName,
          phoneNumber: phoneNumber.trim() || null,
          email: email.trim() || null,
        },
      }).unwrap();
      setSaved(true);
    } catch {
      // The API error is rendered below the form.
    }
  };

  if (!Number.isSafeInteger(id) || id < 1) {
    return (
      <ErrorState error={{ data: { detail: 'The supplier ID must be a positive number.' } }} />
    );
  }
  if (supplier.isLoading) return <LoadingState message="Loading supplier…" />;
  if (supplier.isError) return <ErrorState error={supplier.error} onRetry={supplier.refetch} />;
  if (!supplier.data) return null;

  const data = supplier.data;
  return (
    <div className="max-w-4xl space-y-7">
      <Button asChild variant="ghost" size="sm">
        <Link to="/owner/suppliers">
          <ArrowLeft size={16} /> Back to suppliers
        </Link>
      </Button>
      <PageHeader
        eyebrow={`SUPPLIER #${data.supplierId}`}
        title={data.name}
        description="Supplier profile and contact information."
      />
      <Card className="p-6 sm:p-8">
        <div className="flex items-start gap-4">
          <span className="grid size-12 place-items-center rounded-xl bg-blue-100 text-blue-700">
            <Building2 size={24} />
          </span>
          <div>
            <h2 className="text-xl font-bold">Supplier details</h2>
            <p className="mt-1 text-sm text-slate-500">
              Created {date(data.createdAtUtc)} · Last updated {date(data.updatedAtUtc)}
            </p>
          </div>
        </div>
        <dl className="mt-8 grid gap-5 border-t border-slate-200 pt-6 sm:grid-cols-2">
          <div>
            <dt className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Phone number
            </dt>
            <dd className="mt-2 flex items-center gap-2 text-sm font-medium">
              <Phone size={16} className="text-slate-400" />
              {data.phoneNumber || 'Not provided'}
            </dd>
          </div>
          <div>
            <dt className="text-xs font-semibold uppercase tracking-wide text-slate-500">Email</dt>
            <dd className="mt-2 flex items-center gap-2 text-sm font-medium">
              <Mail size={16} className="text-slate-400" />
              {data.email || 'Not provided'}
            </dd>
          </div>
        </dl>
      </Card>
      <Card className="p-6 sm:p-8">
        <h2 className="text-xl font-bold">Edit supplier</h2>
        <p className="mt-1 text-sm text-slate-500">Update this supplier’s contact information.</p>
        <form
          className="mt-7 grid gap-5 sm:grid-cols-2"
          noValidate
          onSubmit={(event) => void submit(event)}
        >
          <div className="sm:col-span-2">
            <Field label="Supplier name">
              <Input
                value={name}
                onChange={(event) => {
                  setSaved(false);
                  setName(event.target.value);
                }}
                required
                maxLength={200}
              />
            </Field>
          </div>
          <Field label="Phone number (optional)">
            <Input
              value={phoneNumber}
              onChange={(event) => {
                setSaved(false);
                setPhoneNumber(event.target.value);
              }}
              type="tel"
            />
          </Field>
          <Field label="Email (optional)">
            <Input
              value={email}
              onChange={(event) => {
                setSaved(false);
                setEmail(event.target.value);
              }}
              type="email"
            />
          </Field>
          <div className="sm:col-span-2 flex flex-wrap items-center gap-4 border-t border-slate-200 pt-5">
            <Button type="submit" disabled={updateStatus.isLoading || !name.trim()}>
              {updateStatus.isLoading ? 'Saving…' : 'Save changes'}
            </Button>
            <FormError error={updateStatus.error} />
            {saved && (
              <p role="status" className="text-sm text-emerald-700">
                Supplier updated successfully.
              </p>
            )}
          </div>
        </form>
      </Card>
    </div>
  );
}
