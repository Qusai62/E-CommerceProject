import * as AlertDialogPrimitive from '@radix-ui/react-alert-dialog';
import { Button } from './button';
import type { ReactNode } from 'react';
export function ConfirmAction({
  trigger,
  title,
  description,
  onConfirm,
  destructive = false,
}: {
  trigger: ReactNode;
  title: string;
  description: string;
  onConfirm: () => void;
  destructive?: boolean;
}) {
  return (
    <AlertDialogPrimitive.Root>
      <AlertDialogPrimitive.Trigger asChild>{trigger}</AlertDialogPrimitive.Trigger>
      <AlertDialogPrimitive.Portal>
        <AlertDialogPrimitive.Overlay className="fixed inset-0 z-50 bg-black/50" />
        <AlertDialogPrimitive.Content className="fixed left-1/2 top-1/2 z-50 w-[min(92vw,420px)] -translate-x-1/2 -translate-y-1/2 rounded-2xl bg-white p-7 shadow-xl">
          <AlertDialogPrimitive.Title className="text-xl font-semibold">
            {title}
          </AlertDialogPrimitive.Title>
          <AlertDialogPrimitive.Description className="mt-2 text-sm leading-6 text-stone-600">
            {description}
          </AlertDialogPrimitive.Description>
          <div className="mt-6 flex justify-end gap-3">
            <AlertDialogPrimitive.Cancel asChild>
              <Button variant="secondary">Cancel</Button>
            </AlertDialogPrimitive.Cancel>
            <AlertDialogPrimitive.Action asChild>
              <Button variant={destructive ? 'destructive' : 'default'} onClick={onConfirm}>
                Confirm
              </Button>
            </AlertDialogPrimitive.Action>
          </div>
        </AlertDialogPrimitive.Content>
      </AlertDialogPrimitive.Portal>
    </AlertDialogPrimitive.Root>
  );
}
