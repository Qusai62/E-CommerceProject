import { ProductResourceLayout } from '../containers/product-resource-layout';
import { ImageManager } from '../components/image-manager';
export function ProductImagesView({productId}:{productId:number}){
 return <ProductResourceLayout productId={productId} title="Product images">{({product})=><ImageManager productId={productId} images={product.images}/>}</ProductResourceLayout>;
}
